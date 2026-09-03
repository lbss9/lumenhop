using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;

namespace Lumenhop;

/// <summary>Owns the live ping loops and the collection bound to the home list.</summary>
public sealed class PingMonitor
{
    public static PingMonitor Instance { get; } = new();

    public ObservableCollection<PingTargetViewModel> Targets { get; } = [];

    public event EventHandler? Updated;

    private readonly Dictionary<string, CancellationTokenSource> _loops = [];
    private readonly object _gate = new();
    private DispatcherQueue? _queue;
    private bool _running;

    public void Start(DispatcherQueue queue)
    {
        if (_running)
            return;

        _queue = queue;
        _running = true;
        var settings = SettingsStore.Load();
        StatusTheme.Palette = settings.Latency.Clone();
        foreach (var target in settings.Targets)
            Upsert(target, persist: false);

        if (!SettingsStore.Exists())
            Persist();
    }

    public void Stop()
    {
        _running = false;
        lock (_gate)
        {
            foreach (var source in _loops.Values)
                source.Cancel();
            _loops.Clear();
        }
        Targets.Clear();
    }

    public void Add(PingTarget target) => Upsert(target, persist: true);

    public void Update(PingTarget target) => Upsert(target, persist: true);

    public void Remove(string id)
    {
        CancelLoop(id);
        var existing = Targets.FirstOrDefault(item => item.Id == id);
        if (existing is not null)
        {
            IconStore.Delete(existing.IconPath);
            Targets.Remove(existing);
        }
        Persist();
        RaiseUpdated();
    }

    public void SetEnabled(string id, bool enabled)
    {
        var vm = Find(id);
        if (vm is null)
            return;

        vm.IsEnabled = enabled;
        if (enabled)
            RestartLoop(vm);
        else
            CancelLoop(vm.Id);

        Persist();
        RaiseUpdated();
    }

    /// <summary>Adds imported targets, skipping hosts already on the list. Returns how many were added.</summary>
    public int ImportTargets(IEnumerable<PingTarget> targets)
    {
        var known = new HashSet<string>(
            Targets.Select(item => item.Host),
            StringComparer.OrdinalIgnoreCase
        );

        var added = 0;
        foreach (var target in targets)
        {
            if (!TargetValidator.IsValid(target) || !known.Add(target.Host))
                continue;

            target.Id = Guid.NewGuid().ToString("N");
            var vm = PingTargetViewModel.FromTarget(target);
            Targets.Add(vm);
            RestartLoop(vm);
            added++;
        }

        if (added > 0)
        {
            Persist();
            RaiseUpdated();
        }
        return added;
    }

    /// <summary>Applies an edited latency palette to every live card.</summary>
    public void ApplyPalette(LatencyPalette palette)
    {
        StatusTheme.Palette = palette.Clone();
        foreach (var vm in Targets)
            vm.RefreshColor();
        RaiseUpdated();
    }

    /// <summary>Turns every target on or off at once.</summary>
    public void SetAllEnabled(bool enabled)
    {
        foreach (var vm in Targets)
        {
            vm.IsEnabled = enabled;
            if (enabled)
                RestartLoop(vm);
            else
                CancelLoop(vm.Id);
        }

        Persist();
        RaiseUpdated();
    }

    private PingTargetViewModel? Find(string id) => Targets.FirstOrDefault(item => item.Id == id);

    private void Upsert(PingTarget target, bool persist)
    {
        if (!TargetValidator.IsValid(target))
            return;

        var vm = Targets.FirstOrDefault(item => item.Id == target.Id);
        if (vm is null)
        {
            vm = PingTargetViewModel.FromTarget(target);
            Targets.Add(vm);
        }
        else
        {
            if (
                IconStore.IsManaged(vm.IconPath)
                && !string.Equals(vm.IconPath, target.IconPath, StringComparison.OrdinalIgnoreCase)
            )
                IconStore.Delete(vm.IconPath);
            vm.CopyFrom(target);
        }

        RestartLoop(vm);
        if (persist)
            Persist();
        RaiseUpdated();
    }

    private void RestartLoop(PingTargetViewModel vm)
    {
        CancelLoop(vm.Id);
        if (!_running || _queue is null || !vm.IsEnabled)
            return;

        var source = new CancellationTokenSource();
        lock (_gate)
            _loops[vm.Id] = source;

        _ = LoopAsync(vm, source.Token);
    }

    private void CancelLoop(string id)
    {
        lock (_gate)
        {
            if (!_loops.Remove(id, out var source))
                return;
            source.Cancel();
            source.Dispose();
        }
    }

    private async Task LoopAsync(PingTargetViewModel vm, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ProbeOnceAsync(vm, cancellationToken);
            var delay = TimeSpan.FromSeconds(Math.Max(vm.PollingSeconds, 1));
            try
            {
                await Task.Delay(delay, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private async Task ProbeOnceAsync(PingTargetViewModel vm, CancellationToken cancellationToken)
    {
        // Only show the probing hint on the very first reading. On later polls the value
        // updates in place, so the latency text never flashes "…" between readings.
        OnUi(() =>
        {
            if (vm.State == PingState.Idle)
                vm.SetProbing();
        });
        var timeout = Math.Clamp(vm.PollingSeconds * 1000 - 200, 800, PingClient.DefaultTimeoutMs);
        var result = await PingClient.ProbeAsync(vm.Host, timeout, cancellationToken);
        var state = PingStatusMapper.FromProbe(result);
        OnUi(() =>
        {
            vm.ApplyProbe(result, state);
            RaiseUpdated();
        });
    }

    private void Persist()
    {
        var settings = SettingsStore.Load();
        settings.Targets = [.. Targets.Select(item => item.ToTarget())];
        SettingsStore.Save(settings);
    }

    private void RaiseUpdated() => Updated?.Invoke(this, EventArgs.Empty);

    private void OnUi(Action action)
    {
        if (_queue is null || _queue.HasThreadAccess)
        {
            action();
            return;
        }

        _queue.TryEnqueue(() => action());
    }
}

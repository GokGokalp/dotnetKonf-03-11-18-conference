start:
	COMPlus_PerfMapEnabled=1 COMPlus_EnableEventLog=1 COMPlus_ZapDisable=1 dotnet $(p)

startallocations:
	ab -n 10000 localhost:5000/api/calculate?nums=100,200,300,400,500

perfrecord:
	sudo perf record -g -p $(pid)

perfreport:
	sudo perf report -g --call-graph

gcrecord:
	sudo lttng create gc-trace -o ./dotnetkonf-trace
	sudo lttng add-context --userspace --type vpid
	sudo lttng add-context --userspace --type vtid
	sudo lttng add-context --userspace --type procname
	sudo lttng enable-event --userspace --tracepoint DotNETRuntime:GCStart*
	sudo lttng enable-event --userspace --tracepoint DotNETRuntime:GCEnd*
	sudo lttng enable-event --userspace --tracepoint DotNETRuntime:GCHeapStats*
	sudo lttng enable-event --userspace --tracepoint DotNETRuntime:GCAllocationTick*
	sudo lttng enable-event --userspace --tracepoint DotNETRuntime:GCTriggered
	sudo lttng start
	sleep 10
	sudo lttng stop
	sudo lttng destroy
	sudo chown -R parallels ./dotnetkonf-trace

gcstats:
	sudo babeltrace ./dotnetkonf-trace | grep GCAllocationTick* | grep 'TypeName = "[^"]*"' -o | sort | uniq -c | sort -n

gcstatsstack:
	sudo /usr/share/bcc/tools/stackcount ./libcoreclr.so:EventXplatEnabledGCAllocationTick* -f > allocs.stacks -D 10

gcstatsstackview:
	sudo /home/parallels/FlameGraph/flamegraph.pl < allocs.stacks > allocs.svg

exrecord:
	sudo lttng create dotnetKonf -o ./dotnetkonf-trace
	sudo lttng add-context --userspace --type vpid
	sudo lttng add-context --userspace --type vtid
	sudo lttng add-context --userspace --type procname
	sudo lttng enable-event -s dotnetKonf -u --tracepoint DotNETRuntime:Exception*
	sudo lttng start
	sleep 10
	sudo lttng stop
	sudo chown -R parallels ./dotnetkonf-trace

lldbcreate:
	lldb-3.6 $(dotnet) --core ./core.$(pid)

findsosplugin:
	find /usr -name libsosplugin.so

findlibcoreclr:
	find /usr -name libcoreclr.so


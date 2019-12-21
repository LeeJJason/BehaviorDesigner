## 1. [Event 触发优化](Event/README.md)
行为树中能接收Unity中各种物理事件和系统事件，事件触发控制用于管理当前行为树是否需要接收这些事件。但是事件触发标志是遍历所有节点通过反射获得的，性能影响较大。

## 2. [Enable 启动优化](Enable/README.md)
行为树中 `EnableBehavior` 的机制在没看到源代码之前一直是个迷，特别是 `startWhenEnabled` 与 `ExternalBehavior` 在控制时。
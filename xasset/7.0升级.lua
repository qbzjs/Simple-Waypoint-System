7.0 开源版 对比之前的 开源版：

1，用了代码以外的东西解决版本管理的冲突和不稳定问题

2，增加了自动分帧机制来让程序运行的更平滑

3，代码结构会更精炼，编辑器 AssetDatabase 加载资源也是异步实现等。

4，下载器做了一些强化

https://xasset.github.io/#/getstarted

6-7 主要是 打包系统 和 清单做了比较大的调整
打包系统选择合适的配置可以比之前快
可以按需跳过依赖分析
另外 因为是真分布式设计
主要是 不要把所有要打包的资源都 陈列到一个编辑器中
这块 体量大起来的时候
也要不少时间
另外，基本没有临时数据的保存
分组的规则没变的话，一般不会出现 一堆配置文件发送变化的情况
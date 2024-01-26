# RandList

<img src="Assest\WindowIcon.png" style="zoom:50%;" />

这是一款随机名单生成器，能够更加快捷地生成名单。

[查看更新日志](./RELEASE NOTE.md)

## 介绍

- 可选择的多名单，名单存在`/RandList/List.ini`中。
- 可定义的生成模式，随机起点：全随机模式，比如：4，6，9，13，17（任意选择）；固定起点：随机一个起点，比如：4，5，6，7，8（选择一个随机的起点，然后依次取序列）。
- 选择适合你的分隔符号：空格、换行符。

...更多功能有待更新！( •̀ ω •́ )✧
## 截图

<img src="Assest\Screenshot-1.png" style="zoom: 33%;" />

<img src="Assest\Screenshot-2.png" style="zoom:33%;" />

<img src="Assest\Screenshot-3.png" style="zoom:33%;" />

## 下载

你可以在[Github Release](https://github.com/ChenYiLins/RandList/releases)界面找到名为`RandList-x64-x.x.x.exe`的直接安装程序和名为`RandList-Portable-x64-x.x.x.zip`便携版压缩包。

运行要求：

- Window10 build 1809版本以上
- x64位系统

## 使用

名单位于`/RandList/List.ini`中，格式如下。现可以通过菜单：文件-导入 自行导入名单。

```ini
[名单名称]
NUM=4	# 名单成员总数
1=张三	# 名单成员1
2=李四	# 名单成员2
3=王五	# 名单成员3
4=赵六	# 名单成员4
```

## 协议

本项目遵守于[GNU 通用公共许可证，版本 2](https://www.gnu.org/licenses/old-licenses/gpl-2.0.html)。

## 赞助

请不要在这拉跨的项目上面浪费了，请赞助那些需要的项目吧。(っ °Д °;)っ

## 题外话

- 关于Windows App SDK开发程序体积的庞大：
  - 不知道基于什么原因，使用.NET 7开发的WAS程序无法使用AOT编译。
  - 在不经过任何的调整的情况下，程序编译出来大概是150MB左右的大小，实在是让人汗颜(╬▔皿▔)╯。
  - 在自行添加裁剪之后，同时在启用ReadyToRun的情况下，程序成功缩减到100MB左右，.NET 7修改了[剪裁粒度](https://learn.microsoft.com/dotnet/core/deploying/trimming/trimming-options?pivots=dotnet-7-0#trimming-granularity)的默认值，所以也需要自行修改，或许该试试.NET 8？其实.NET 6已经可以轻松实现编译到100MB以下的大小，至少在我自己的[GoTool](https://github.com/ChenYiLins/GoTool)项目中有所体现(* ￣︿￣)。
  - 更大的原因是.Net Core和WAS的单独打包？
- 感谢Windows App SDK的不懈努力下，让我至今没有用上一个很舒服TitleBar：
  - 当然这也是能理解的，毕竟WAS至今没有提供一个比较理想的性能表现，或者这也是Explorer现在不伦不类的原因，开发者很想提供一个统一的界面，但是得益于WinUI的性能表现，祖宗之法不可弃也。
  - TitleBar的按钮颜色，我觉得Win11提供的设置程序也是错误的设计，我曾在[WinUI 3 Galley的Issues](https://github.com/microsoft/WinUI-Gallery/issues/1364)中提到过。我不觉得在暗色的背景下提供一个接近黑色的Hover背景是正确的（R：25，G：25，B：25），在Win11的系统应用中，计算器程序应该是给出了一个正确的示例。
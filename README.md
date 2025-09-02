# DapLinkFlashSPI

### 简介

这是一个用CMSIS-DAP烧写SPI FLASH的上位机小工具。
GUI使用WinForm，USB驱动使用UWP，所以只支持Win10平台。
使用json文件配置不同FLASH的指令，以期望实现最大的兼容性。

### 实现原理

- DAP_JTAG_Sequence命令的数据包格式类似SPI操作，而且可以控制TMS的电平，还可以选择是否捕获TDO输入，非常容易模拟SPI时序。
- 本软件只使用JTAG_Sequence，拉高、拉低TMS使用额外的一个Byte而不是发出SWJ_Pins指令，因为有的调试器可能不支持SWJ_Pins。
- JTAG_Sequence的每个Sequence最大长度是64个bit，即8Byte。为了加快速度，我们在一个数据包内填充多个Sequence。
- JTAG的数据传输是LSB格式，而FLASH要求数据是MSB格式，所以每个实际的SPI数据Byte都要进行MSB->LSM翻转，软件内使用查表法计算。
- JTAG_Sequence可以选择是否捕获TDO输入数据。为了方便提取数据，我们只在数据有意义时捕获输入，命令、地址部分不捕获输入。

### DAP兼容性问题

1. 在一个DAP_JTAG_Sequence命令内，TMS必须始终维持上一次的写入值，不能在数据Byte的间隔内产生跳动（某些使用SPI模拟SWD的DAP可能会在切换IO模式时产生跳动），否则会导致FLASH检测到CS拉高，导致命令中断。

### 添加、修改FLASH配置

1. 配置文件是ConfigDapLinkFlashSPI.json，编译后手动复制到二进制文件夹内，或者自行打包。
2. 配置文件的每一项必须要填写，不然会解析出null导致软件崩溃。

### bug、缺陷

1. 时钟频率使用了固定的10M，没有实现选择功能。
2. C#水平差劲。
3. 可能需要更多配置来兼容不同的FLASH。

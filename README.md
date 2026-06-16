# 歇歇眼睛 (XiexieEyes)

多屏全屏锁定版久坐提醒工具。基于 [Sedentary-reminder](https://github.com/wjbgis/Sedentary-reminder) 改进。

## 功能特性

- **多屏全屏覆盖** — 多显示器环境下每屏独立全屏遮罩
- **健康提醒文案** — 人民日报久坐危害数据
- **定时锁屏提示** — 自定义休息间隔
- **键盘输入锁定** — 休息期间屏蔽输入

## 使用说明

双击 `release/歇歇眼睛.exe` 运行。

## 构建

```
msbuild src/Reminder/歇歇眼睛.csproj /p:Configuration=Release /p:Platform=AnyCPU
```

## 许可

MIT

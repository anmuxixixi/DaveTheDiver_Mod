# 未收集昆虫刷新 Mod 详细安装指南

[返回仓库首页](../README.md) · [查看功能介绍与刷新规则](UNCOLLECTED_INSECT_SPAWNER.zh-CN.md)

本教程只适用于“未收集昆虫刷新 Mod”，其安装包名称为 `UncollectedBeetleSpawner-vX.Y.Z.zip`。不要误装甲虫大作战预测器的 `BeetleBattlePredictor` 安装包。

## 安装前准备

- Windows 10 或 Windows 11 64 位
- Steam 版《潜水员戴夫》及丛林 DLC
- 安装过程中保持游戏完全退出
- BepInEx 6 IL2CPP Windows x64；本项目验证版本为 `6.0.0-be.785`

## 第一步：找到游戏根目录

在 Steam 游戏库中右键《潜水员戴夫》，选择“管理”→“浏览本地文件”。确认打开的目录中存在：

```text
DaveTheDiver.exe
```

后续提到的“游戏根目录”就是这个目录。

## 第二步：安装 BepInEx 6 IL2CPP

如果已经为其他 Mod 正确安装了 BepInEx 6 IL2CPP，可以跳过本步。

[下载 BepInEx 6.0.0-be.785（Windows x64 IL2CPP）](https://builds.bepinex.dev/projects/bepinex_be/785/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.785%2B6abdba4.zip)

1. 解压 BepInEx ZIP。
2. 把压缩包内部的所有文件复制到游戏根目录，与 `DaveTheDiver.exe` 同级。
3. 从 Steam 启动一次游戏，进入主菜单后正常退出。
4. 确认已经生成 `BepInEx\LogOutput.log` 和 `BepInEx\interop`。

## 第三步：下载正确的 Mod 包

[下载 UncollectedBeetleSpawner v1.0.10](https://raw.githubusercontent.com/anmuxixixi/DaveTheDiver_Mod/main/artifacts/UncollectedBeetleSpawner-v1.0.10.zip)

文件名必须是：

```text
UncollectedBeetleSpawner-v1.0.10.zip
```

`BeetleBattlePredictor-v1.0.8.zip` 是另一个甲虫对战预测 Mod，不是本教程要安装的文件。

## 第四步：安装 Mod

1. 完全退出游戏。
2. 解压 `UncollectedBeetleSpawner-v1.0.10.zip`。
3. 将其中的 `BepInEx` 文件夹合并到游戏根目录。
4. 确认最终文件位于：

```text
游戏根目录\BepInEx\plugins\UncollectedBeetleSpawner\UncollectedBeetleSpawner.dll
```

不要形成下面这种多套一层的错误路径：

```text
游戏根目录\BepInEx\plugins\BepInEx\plugins\UncollectedBeetleSpawner\...
```

## 第五步：验证加载

启动游戏后，本 Mod 会自动启用，不需要按 `F8`。退出游戏并打开 `BepInEx\LogOutput.log`，搜索：

```text
Uncollected Beetle Spawner 1.0.10 loaded.
```

如果同时安装了甲虫大作战预测 Mod，日志中还会出现它自己的加载记录。这是正常现象，两个 Mod 使用不同目录，不会互相覆盖。

## 使用 Codex 安装

可以把下面这段话单独发送给 Codex：

```text
请从 https://github.com/anmuxixixi/DaveTheDiver_Mod 下载并安装未收集昆虫刷新 Mod v1.0.10。只使用 artifacts/UncollectedBeetleSpawner-v1.0.10.zip，不要安装 BeetleBattlePredictor。确认游戏已经退出，将压缩包内的 BepInEx 文件夹合并到《潜水员戴夫》游戏根目录，最后报告 DLL 版本和 SHA256。
```

## 更新

1. 退出游戏。
2. 下载版本号更高的 `UncollectedBeetleSpawner-vX.Y.Z.zip`。
3. 按“第四步”覆盖旧 DLL。
4. 不需要删除 BepInEx 或重新生成 `interop`。

## 卸载

退出游戏后，只删除：

```text
游戏根目录\BepInEx\plugins\UncollectedBeetleSpawner
```

不要删除整个 `BepInEx` 文件夹，否则其他 Mod 也会失效。

## 常见问题

### 安装后完全没有变化

检查 DLL 路径及日志中的加载记录。本 Mod 只影响丛林探索地图的甲虫和蝴蝶刷新，不会在甲虫大作战界面显示预测信息。

### 仍然刷出已经收集的昆虫

先确认使用的是 v1.0.10，并保留 `BepInEx\LogOutput.log`。日志会记录刷新编号、图鉴物品编号和候选收集状态，可用于判断存档记录或游戏版本变化。

### 特定蝴蝶没有出现

固定蝴蝶仍受原版地图、剧情和天气限制。请先确认当前条件原本允许该物种出现；本 Mod 不会绕过这些限制。

### 安装后游戏闪退

1. 删除 `BepInEx\plugins\UncollectedBeetleSpawner`，确认游戏能否恢复启动。
2. 检查是否使用 BepInEx 6 IL2CPP Windows x64，而不是 BepInEx 5 或 Mono 版。
3. 保留 `BepInEx\LogOutput.log` 的最后一段错误信息用于排查。

# 甲虫大作战预测 Mod 详细安装指南

本指南适用于没有安装 Codex、希望手动安装 Mod 的普通 Windows Steam 玩家。

## 安装前准备

- Windows 10 或 Windows 11 64 位系统
- Steam 版《潜水员戴夫》及丛林 DLC
- 能够正常启动游戏
- 安装过程中保持游戏完全退出

本 Mod 不修改存档、血量、伤害或战斗结算，但安装第三方 Mod 前仍建议保留重要存档的备份。

## 第一步：找到游戏目录

1. 打开 Steam 游戏库。
2. 右键《潜水员戴夫》。
3. 选择“管理”→“浏览本地文件”。
4. Steam 会打开游戏根目录。确认目录内可以看到 `DaveTheDiver.exe`。

常见默认位置如下，实际路径可能因 Steam 库位置不同而变化：

```text
C:\Program Files (x86)\Steam\steamapps\common\Dave the Diver
```

后续步骤中提到的“游戏根目录”就是包含 `DaveTheDiver.exe` 的这个目录。

## 第二步：安装 BepInEx 6 IL2CPP

本 Mod 不能脱离 BepInEx 单独运行。请使用本项目验证过的 Windows x64 IL2CPP 版本：

[下载 BepInEx 6.0.0-be.785（Windows x64 IL2CPP）](https://builds.bepinex.dev/projects/bepinex_be/785/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.785%2B6abdba4.zip)

1. 下载完成后，右键 ZIP 文件并选择“全部解压”。
2. 打开解压后的文件夹。
3. 将里面的所有文件和文件夹复制到游戏根目录，而不是复制 ZIP 文件本身。
4. 如果 Windows 询问是否合并文件夹，请选择允许。

安装正确后，目录结构应类似：

```text
Dave the Diver\
├─ DaveTheDiver.exe
├─ BepInEx\
├─ dotnet\
├─ doorstop_config.ini
└─ winhttp.dll
```

如果出现 `Dave the Diver\BepInEx-Unity.IL2CPP-win-x64-...\BepInEx` 这种多套一层文件夹的结构，说明复制位置不正确。应把压缩包内部内容直接放到 `DaveTheDiver.exe` 旁边。

## 第三步：首次启动 BepInEx

1. 从 Steam 启动一次游戏。
2. 第一次启动可能需要等待几分钟，BepInEx 会生成 IL2CPP 互操作文件。此时不要强制结束游戏。
3. 进入游戏主菜单后正常退出游戏。
4. 回到游戏根目录，确认以下文件或目录已经生成：

```text
BepInEx\LogOutput.log
BepInEx\interop\
```

如果两者都不存在，说明 BepInEx 尚未正确加载，请先查看本指南末尾的“常见问题”。

## 第四步：下载 Mod

打开仓库的 [artifacts 发布包目录](https://github.com/anmuxixixi/DaveTheDiver_Mod/tree/main/artifacts)，选择版本号最高的 `BeetleBattlePredictor-vX.Y.Z.zip`。

当前版本可直接下载：

[下载 BeetleBattlePredictor v1.0.8](https://raw.githubusercontent.com/anmuxixixi/DaveTheDiver_Mod/main/artifacts/BeetleBattlePredictor-v1.0.8.zip)

如果浏览器只显示下载页面，请使用页面右上角的下载按钮保存 ZIP 文件。

## 第五步：安装 Mod 文件

1. 确认游戏已经退出。
2. 解压 `BeetleBattlePredictor-vX.Y.Z.zip`。
3. 将压缩包内的 `BepInEx` 文件夹复制到游戏根目录。
4. Windows 询问时选择合并文件夹和覆盖同名 Mod DLL。

最终必须存在下面这个文件：

```text
游戏根目录\BepInEx\plugins\BeetleBattlePredictor\BeetleBattlePredictor.dll
```

请特别检查没有出现以下错误的嵌套路径：

```text
游戏根目录\BepInEx\plugins\BepInEx\plugins\BeetleBattlePredictor\...
```

## 第六步：进入游戏使用

1. 从 Steam 正常启动游戏。
2. 进入丛林 DLC 的甲虫大作战。
3. 每次启动游戏时，预测功能默认关闭。
4. 按 `F8` 开启预测；再次按 `F8` 可以关闭。
5. 选择出招时，左侧面板会显示对手招式和建议的克制动作。

出招对应关系：

| 对手动作 | 猜拳含义 | 玩家应对 |
|---|---|---|
| 冲锋 | 石头 | 防御/回血（布） |
| 角攻击 | 剪刀 | 冲锋（石头） |
| 防御/回血 | 布 | 角攻击（剪刀） |

## 更新 Mod

1. 退出游戏。
2. 从 `artifacts` 目录下载版本号最高的新 ZIP。
3. 按“第五步”重新复制 `BepInEx` 文件夹并覆盖旧 DLL。
4. 原有配置会保留，无需删除 BepInEx 或重新生成 `interop`。

启动后可以在 `BepInEx\LogOutput.log` 中搜索以下内容确认版本：

```text
Beetle Battle Predictor 1.0.8 loaded
```

## 卸载 Mod

退出游戏后删除以下文件夹：

```text
游戏根目录\BepInEx\plugins\BeetleBattlePredictor
```

如果还想删除本 Mod 的配置，可额外删除：

```text
游戏根目录\BepInEx\config\cn.codex.davethediver.beetlebattlepredictor.cfg
```

如果还安装了其他 Mod，不要删除整个 `BepInEx` 文件夹，否则其他 Mod 也会失效。

## 常见问题

### 按 F8 没有任何提示

依次检查：

1. DLL 是否位于正确的 `BepInEx\plugins\BeetleBattlePredictor` 路径。
2. `BepInEx\LogOutput.log` 中是否包含 `Beetle Battle Predictor 1.0.8 loaded`。
3. 是否误装了 BepInEx 5、Mono 版或 32 位版；本游戏需要 BepInEx 6 IL2CPP Windows x64。
4. 笔记本键盘是否需要同时按 `Fn+F8`。

### 游戏第一次启动很慢

首次安装 BepInEx 后需要生成 IL2CPP 互操作文件，等待时间会比平时长。只要进程仍在运行，就先耐心等待。

### 一直显示“正在锁定对手招式”

确认使用的是仓库中版本号最高的安装包。如果游戏刚更新，内部类或方法签名可能已经变化。请保留 `BepInEx\LogOutput.log`，并在仓库反馈游戏 Steam Build ID 和日志。

### 面板遮挡游戏 UI

v1.0.7 及更高版本的默认位置已经下移。若曾手动修改过配置，可打开：

```text
BepInEx\config\cn.codex.davethediver.beetlebattlepredictor.cfg
```

将 `[Overlay]` 下的 `Y` 调整为 `340` 或更大的数值。数值越大，面板越靠下。

### 安装后游戏无法启动

1. 删除 `BepInEx\plugins\BeetleBattlePredictor`，确认游戏能否恢复启动。
2. 检查 BepInEx 是否为本指南指定的 IL2CPP x64 版本。
3. 查看 `BepInEx\LogOutput.log` 最后出现的错误。
4. 不要混用 BepInEx 5 与 BepInEx 6 文件。

需要反馈问题时，请附上游戏 Steam Build ID、Mod 版本以及 `BepInEx\LogOutput.log`，但先检查日志中是否包含个人路径或其他不希望公开的信息。

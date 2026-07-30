# 甲虫大作战预测 Mod

适用于《潜水员戴夫：丛林》Windows Steam 版的 BepInEx 6 IL2CPP 插件。它会在玩家确认前提前生成并锁定敌方甲虫出招，在选择阶段显示对手动作和必胜克制动作。

本 Mod 会改变敌方 AI 的出招生成时机，但不修改血量、伤害、战斗结算或存档。

## 对应关系

| 游戏内部动作 | 猜拳 | 应对 |
|---|---|---|
| Defense（防御/回血） | 布 | 出剪刀（角攻击） |
| HornAttack（角攻击） | 剪刀 | 出石头（冲锋） |
| Rush（冲锋） | 石头 | 出布（防御/回血） |

## 安装

1. 安装 Windows x64 的 BepInEx 6 IL2CPP。本项目使用并验证了 `BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.785+6abdba4.zip`。
2. 将 BepInEx 压缩包内容解压到游戏根目录（与 `DaveTheDiver.exe` 同级）。
3. 启动一次游戏再退出，让 BepInEx 生成 `BepInEx\interop`。
4. 将发布包中的 `BepInEx` 文件夹合并到游戏根目录；或在本项目中运行 `powershell -ExecutionPolicy Bypass -File .\install.ps1 -GameDir '游戏目录'`。
5. 每次启动默认关闭。进入甲虫战斗后按 `F8` 开启，对手招式会在确认出招前显示并锁定。

游戏目录可在 Steam 中通过“管理 → 浏览本地文件”打开。

## 编译

```powershell
.\build.ps1 -GameDir '你的游戏目录'
```

成品位于 `artifacts\BeetleBattlePredictor-v1.0.6.zip`。

项目优先引用游戏首次启动后生成的 `BepInEx\interop`；若尚未安装 BepInEx，则开发环境可使用元数据分析产生的兼容引用程序集。

## 配置与排错

配置文件首次运行后生成在 `BepInEx\config\cn.codex.davethediver.beetlebattlepredictor.cfg`，可调整开关、位置和字号。

若没有提示，请检查 `BepInEx\LogOutput.log` 中是否出现 `Beetle Battle Predictor 1.0.6 loaded`。游戏更新后如果类名或方法签名变化，请附上该日志与游戏 Build ID 报告问题。当前代码按 Steam Build ID `24077479` 分析。

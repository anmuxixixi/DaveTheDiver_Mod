# 甲虫大作战预测 Mod

## 效果预览

![甲虫大作战预测 Mod 效果：显示对手招式与推荐应对动作](docs/images/beetle-battle-predictor-preview.png)

开启后，左上角会显示对手即将使用的招式，以及对应的必胜应对动作。上图中对手将出“石头·冲锋”，Mod 建议使用“布·防御/回血”。

本项目适用于《潜水员戴夫：丛林》Windows Steam 版，是一个 BepInEx 6 IL2CPP 插件。它会在玩家确认前提前生成并锁定敌方甲虫出招，在选择阶段显示对手动作和必胜克制动作。

本 Mod 会改变敌方 AI 的出招生成时机，但不修改血量、伤害、战斗结算或存档。

## 快速开启和关闭

- 每次启动游戏时，预测功能默认关闭。
- 进入甲虫大作战后按 `F8` 开启，屏幕会短暂显示“甲壳虫预测：已开启”。
- 再按一次 `F8` 即可关闭，屏幕会短暂显示“甲壳虫预测：已关闭”。
- 部分笔记本电脑需要按 `Fn+F8`。

## 对应关系

| 游戏内部动作 | 猜拳 | 应对 |
|---|---|---|
| Defense（防御/回血） | 布 | 出剪刀（角攻击） |
| HornAttack（角攻击） | 剪刀 | 出石头（冲锋） |
| Rush（冲锋） | 石头 | 出布（防御/回血） |

## 使用 Codex 自动安装

已安装 Codex 的 Windows 玩家，可以把下面这段话直接粘贴给 Codex：

```text
请从 https://github.com/anmuxixixi/DaveTheDiver_Mod 安装最新版甲虫大作战预测 Mod。下载并检查仓库中的 install-from-github.ps1，然后运行它；让脚本自动定位 Steam 游戏目录、安装缺失的 BepInEx 6 IL2CPP 和最新版 Mod。执行外部下载及写入游戏目录前向我申请权限，完成后报告安装版本和 DLL SHA256。
```

Codex 会在需要访问 GitHub、BepInEx 官方下载站或写入游戏目录时显示权限确认。安装器不会在游戏运行时覆盖文件，并且只从本仓库的 `main` 分支选择版本号最高的发布包。

也可以在克隆本仓库后直接运行：

```powershell
.\install-from-github.ps1
```

如果自动定位失败，可明确指定游戏目录：

```powershell
.\install-from-github.ps1 -GameDir 'E:\SteamLibrary\steamapps\common\Dave the Diver'
```

## 手动安装

没有安装 Codex、需要逐步操作说明的玩家，请阅读：

**[普通玩家详细安装指南（BepInEx 安装、Mod 安装、更新、卸载与排错）](docs/INSTALL.zh-CN.md)**

简要步骤：

1. 安装 Windows x64 的 BepInEx 6 IL2CPP。本项目使用并验证了 `BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.785+6abdba4.zip`。
2. 将 BepInEx 压缩包内容解压到游戏根目录（与 `DaveTheDiver.exe` 同级）。
3. 启动一次游戏再退出，让 BepInEx 生成 `BepInEx\interop`。
4. 将发布包中的 `BepInEx` 文件夹合并到游戏根目录。
5. 进入甲虫大作战后按 `F8` 开启预测。

游戏目录可在 Steam 中通过“管理 → 浏览本地文件”打开。

# 潜水员戴夫 Mod 合集

本仓库包含两个彼此独立的《潜水员戴夫：丛林》Mod。它们解决的问题、使用方式和安装包都不同，可以单独安装，也可以同时安装。

> ❗❗❗**声明：仅用作Mod学习，不得商用**

| Mod | 用途 | 当前版本 | 游戏内操作 |
|---|---|---:|---|
| 甲虫大作战预测 Mod | 在甲虫大作战中显示对手下一招和推荐克制动作 | v1.0.8 | 进入对战后按 `F8` 开关 |
| 昆虫全收集福音 Mod | 在丛林探索中优先刷新尚未收集的甲虫和蝴蝶 | v1.0.10 | 安装后自动启用 |

> “甲虫大作战预测 Mod”只影响甲虫对战；“未收集昆虫刷新 Mod”只影响探索地图中的昆虫刷新。请根据名称下载对应 ZIP，不要混用安装包或 DLL。

## 甲虫大作战预测 Mod

![甲虫大作战预测 Mod 效果：显示对手招式与推荐应对动作](docs/images/beetle-battle-predictor-preview.png)

技术名称：`BeetleBattlePredictor`

- [功能介绍与使用教程](docs/BEETLE_BATTLE_PREDICTOR.zh-CN.md)
- [独立安装、更新、卸载与排错教程](docs/INSTALL_BEETLE_BATTLE_PREDICTOR.zh-CN.md)
- [下载 BeetleBattlePredictor v1.0.8](https://raw.githubusercontent.com/anmuxixixi/DaveTheDiver_Mod/main/artifacts/BeetleBattlePredictor-v1.0.8.zip)

安装后的 DLL 路径：

```text
游戏根目录\BepInEx\plugins\BeetleBattlePredictor\BeetleBattlePredictor.dll
```

## 昆虫全收集福音 Mod

> Q：能解决什么需求？
> 
> A：对于昆虫（蝴蝶&甲虫）全收集玩家，每次昆虫刷出的时机都不一样，有时候这棵树会刷甲虫，有时候不刷；特别是英雄翠凤蝶只在晴天或阴天在独木桥附近刷出来，得看脸😭
> 
> 为此，这个Mod就是解决这个烦恼；**他会在刷出点100%刷出未收集的昆虫**，如果已经全收集还是按照游戏原来的逻辑随机生成昆虫

技术名称：`UncollectedBeetleSpawner`。为兼容已经安装的版本，DLL 和插件目录继续沿用这个技术名称，但 v1.0.10 已同时支持甲虫与蝴蝶。

- [功能介绍与刷新规则说明](docs/UNCOLLECTED_INSECT_SPAWNER.zh-CN.md)
- [独立安装、更新、卸载与排错教程](docs/INSTALL_UNCOLLECTED_INSECT_SPAWNER.zh-CN.md)
- [下载 UncollectedBeetleSpawner v1.0.10](https://raw.githubusercontent.com/anmuxixixi/DaveTheDiver_Mod/main/artifacts/UncollectedBeetleSpawner-v1.0.10.zip)

安装后的 DLL 路径：

```text
游戏根目录\BepInEx\plugins\UncollectedBeetleSpawner\UncollectedBeetleSpawner.dll
```

## 共同运行环境

- Windows 10 或 Windows 11 64 位
- Steam 版《潜水员戴夫》及丛林 DLC
- BepInEx 6 IL2CPP Windows x64
- 本项目使用并验证了 `BepInEx 6.0.0-be.785`

两个 Mod 使用不同的插件目录和程序集名称，不会互相覆盖。安装或更新前请完全退出游戏；如果还安装了其他 Mod，卸载时只删除目标 Mod 自己的插件目录，不要删除整个 `BepInEx`。

## 开发与构建

构建需要游戏已经生成的 `BepInEx\interop` 程序集：

```powershell
# 构建甲虫大作战预测 Mod
.\build.ps1 -GameDir '你的游戏目录'

# 构建未收集昆虫刷新 Mod
.\build-uncollected-beetles.ps1 -GameDir '你的游戏目录'
```

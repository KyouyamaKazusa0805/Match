# 连连看游戏

提供一组 API 可以操作连连看游戏（一种中国盛行的益智游戏，类似麻将匹配，不过是平面的，且需要找出盘面里可消除的一对相同元素，且必须在不超过两次拐弯就能互相看见的才行）。

## 使用方式

### 生成一个题目

如果要出一个题目，请使用 `Generator.Generate` 方法来出题：

```csharp
using System;
using Match.Generating;

var generator = new Generator();
var grid = generator.Generate(10, 10, 30);
Console.WriteLine(grid.ToString());
```

一个生成的题目如下：

```text
[
  04, 19, 29, 28, 07, 18, 09, 20, 25, 01,
  27, 13, 04, 07, 05, 12, 27, 05, 04, 05,
  17, 09, 11, 24, 11, 25, 13, 29, 15, 27,
  05, 04, 14, 18, 23, 01, 15, 21, 12, 06,
  26, 02, 06, 10, 04, 12, 09, 13, 13, 01,
  28, 28, 03, 17, 20, 04, 23, 10, 13, 13,
  28, 25, 04, 08, 21, 00, 00, 27, 01, 09,
  29, 16, 22, 09, 28, 18, 09, 09, 28, 04,
  24, 29, 01, 26, 01, 04, 09, 00, 12, 02,
  17, 18, 03, 14, 25, 28, 16, 04, 19, 08
]
```

### 寻求盘面的可用步骤

你可以使用 `Grid` 类型里的 `GetAllMatches`、`GetMatch` 和 `TryGetMatch` 来获得可用步骤：

```csharp
using System;
using Match.Concepts;

// 使用字符串解析一个题目。
// 由于游戏的设计，你需要在盘面的序列之前加上一个尺寸记号，格式是“<行数>:<列数>:<盘面数据>”。
// 不过，因为有给予行列数，所以盘面的数据可以不换行。
var grid = Grid.Parse(
    """
    10:10:[
        04, 19, 29, 28, 07, 18, 09, 20, 25, 01,
        27, 13, 04, 07, 05, 12, 27, 05, 04, 05,
        17, 09, 11, 24, 11, 25, 13, 29, 15, 27,
        05, 04, 14, 18, 23, 01, 15, 21, 12, 06,
        26, 02, 06, 10, 04, 12, 09, 13, 13, 01,
        28, 28, 03, 17, 20, 04, 23, 10, 13, 13,
        28, 25, 04, 08, 21, 00, 00, 27, 01, 09,
        29, 16, 22, 09, 28, 18, 09, 09, 28, 04,
        24, 29, 01, 26, 01, 04, 09, 00, 12, 02,
        17, 18, 03, 14, 25, 28, 16, 04, 19, 08
    ]
    """);

// 你可以通过 'GetAllMatches'、'TryGetMatch' 和 'GetMatch' 来获取可用步骤。
// 不过，'GetAllMatches' 是获取全部的可用步骤，
// 而 'TryGetMatch' 和 'GetMatch' 只返回一个找到的步骤。
var matches = grid.GetAllMatches();
foreach (var match in matches)
{
    Console.WriteLine(match.ToString());
}
```

全部找到的步骤如下：

```text
ItemMatch { Start = Coordinate { X = 6, Y = 5 }, End = Coordinate { X = 6, Y = 6 }, Interims = [] }
ItemMatch { Start = Coordinate { X = 0, Y = 9 }, End = Coordinate { X = 4, Y = 9 }, Interims = [Coordinate { X = 0, Y = 10 }, Coordinate { X = 4, Y = 10 }] }
ItemMatch { Start = Coordinate { X = 7, Y = 6 }, End = Coordinate { X = 8, Y = 6 }, Interims = [] }
ItemMatch { Start = Coordinate { X = 7, Y = 6 }, End = Coordinate { X = 7, Y = 7 }, Interims = [] }
ItemMatch { Start = Coordinate { X = 5, Y = 8 }, End = Coordinate { X = 5, Y = 9 }, Interims = [] }
ItemMatch { Start = Coordinate { X = 4, Y = 8 }, End = Coordinate { X = 5, Y = 8 }, Interims = [] }
ItemMatch { Start = Coordinate { X = 4, Y = 7 }, End = Coordinate { X = 4, Y = 8 }, Interims = [] }
ItemMatch { Start = Coordinate { X = 2, Y = 0 }, End = Coordinate { X = 9, Y = 0 }, Interims = [Coordinate { X = 2, Y = -1 }, Coordinate { X = 9, Y = -1 }] }
ItemMatch { Start = Coordinate { X = 5, Y = 0 }, End = Coordinate { X = 6, Y = 0 }, Interims = [] }
ItemMatch { Start = Coordinate { X = 5, Y = 0 }, End = Coordinate { X = 5, Y = 1 }, Interims = [] }
```


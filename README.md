# Match

A project that can operate with match game (finding a pair of same cards in a matrix with links not turning > 2 times).

## Usage

### Generate a puzzle

To generate a puzzle, you can use `Generator.Generate` to create a new puzzle:

```csharp
using System;
using Match.Generating;

var generator = new Generator();
var grid = generator.Generate(10, 10, 30);
Console.WriteLine(grid.ToString());
```

A generated puzzle can be:

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

### Provide suggestions of paired items

You can directly use methods  inside `Grid` to fetch suggestions:

```csharp
using System;
using Match.Concepts;

// To parse a puzzle.
// Due to design of API, size is a required information to calculate a puzzle.
// You can append "size marker" before the grid string, and the format is "<row>:<column>:<grid-data>".
// Please note that, "<grid-data>" part may not require a multi-line string.
// You can just use a single-line string as a valid input.
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

// To fetch suggestions, use methods 'GetAllMatches', 'TryGetMatch' or 'GetMatch'.
// 'GetAllMatches' will return all possible matches appeared in the current grid,
// while 'TryGetMatch' and 'GetMatch' only returns the first found suggestion.
var matches = grid.GetAllMatches();
foreach (var match in matches)
{
    Console.WriteLine(match.ToString());
}
```

All found matches are:

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


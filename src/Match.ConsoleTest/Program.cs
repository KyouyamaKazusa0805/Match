using System;
using Match.Generating;

var grid1 = Generator.Generate(6, 6, 12);
var grid2 = Generator.Generate(6, 6, 12);
var grid3 = Generator.Generate(6, 6, 12);
Console.WriteLine(grid1!.ToString());
Console.WriteLine(grid2!.ToString());
Console.WriteLine(grid3!.ToString());

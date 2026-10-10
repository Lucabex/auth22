using System.Text.Json.Serialization;

namespace auth22.Records;

public record DailyPuzzle(
    [property:JsonPropertyName("puzzle")]DailyInfo Puzzle

);

public record DailyInfo(

    [property:JsonPropertyName("solution")]List<string> Solution,
     [property:JsonPropertyName("themes")]List<string> Themes,
     [property:JsonPropertyName("fen")]string Fen
);
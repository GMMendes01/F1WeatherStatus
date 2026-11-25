namespace F1WeatherStatus.Models;

/// <summary>Uma linha do ranking de títulos.</summary>
public class ItemRanking
{
    public int Posicao { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Titulos { get; set; }
    public string Anos { get; set; } = string.Empty;

    public string PosicaoTexto => $"{Posicao}º";
    public string TitulosTexto => Titulos == 1 ? "1 título" : $"{Titulos} títulos";
}

/// <summary>
/// Campeões de cada temporada, copiados da API Jolpica (api.jolpi.ca) até 2025.
/// Ficam no app porque a API exige uma requisição por ano e limita a velocidade.
/// </summary>
public static class HistoricoCampeoes
{
    public static readonly (int Ano, string Nome)[] Pilotos =
    {
        (1950, "Nino Farina"),
        (1951, "Juan Fangio"),
        (1952, "Alberto Ascari"),
        (1953, "Alberto Ascari"),
        (1954, "Juan Fangio"),
        (1955, "Juan Fangio"),
        (1956, "Juan Fangio"),
        (1957, "Juan Fangio"),
        (1958, "Mike Hawthorn"),
        (1959, "Jack Brabham"),
        (1960, "Jack Brabham"),
        (1961, "Phil Hill"),
        (1962, "Graham Hill"),
        (1963, "Jim Clark"),
        (1964, "John Surtees"),
        (1965, "Jim Clark"),
        (1966, "Jack Brabham"),
        (1967, "Denny Hulme"),
        (1968, "Graham Hill"),
        (1969, "Jackie Stewart"),
        (1970, "Jochen Rindt"),
        (1971, "Jackie Stewart"),
        (1972, "Emerson Fittipaldi"),
        (1973, "Jackie Stewart"),
        (1974, "Emerson Fittipaldi"),
        (1975, "Niki Lauda"),
        (1976, "James Hunt"),
        (1977, "Niki Lauda"),
        (1978, "Mario Andretti"),
        (1979, "Jody Scheckter"),
        (1980, "Alan Jones"),
        (1981, "Nelson Piquet"),
        (1982, "Keke Rosberg"),
        (1983, "Nelson Piquet"),
        (1984, "Niki Lauda"),
        (1985, "Alain Prost"),
        (1986, "Alain Prost"),
        (1987, "Nelson Piquet"),
        (1988, "Ayrton Senna"),
        (1989, "Alain Prost"),
        (1990, "Ayrton Senna"),
        (1991, "Ayrton Senna"),
        (1992, "Nigel Mansell"),
        (1993, "Alain Prost"),
        (1994, "Michael Schumacher"),
        (1995, "Michael Schumacher"),
        (1996, "Damon Hill"),
        (1997, "Jacques Villeneuve"),
        (1998, "Mika HÃ¤kkinen"),
        (1999, "Mika HÃ¤kkinen"),
        (2000, "Michael Schumacher"),
        (2001, "Michael Schumacher"),
        (2002, "Michael Schumacher"),
        (2003, "Michael Schumacher"),
        (2004, "Michael Schumacher"),
        (2005, "Fernando Alonso"),
        (2006, "Fernando Alonso"),
        (2007, "Kimi RÃ¤ikkÃ¶nen"),
        (2008, "Lewis Hamilton"),
        (2009, "Jenson Button"),
        (2010, "Sebastian Vettel"),
        (2011, "Sebastian Vettel"),
        (2012, "Sebastian Vettel"),
        (2013, "Sebastian Vettel"),
        (2014, "Lewis Hamilton"),
        (2015, "Lewis Hamilton"),
        (2016, "Nico Rosberg"),
        (2017, "Lewis Hamilton"),
        (2018, "Lewis Hamilton"),
        (2019, "Lewis Hamilton"),
        (2020, "Lewis Hamilton"),
        (2021, "Max Verstappen"),
        (2022, "Max Verstappen"),
        (2023, "Max Verstappen"),
        (2024, "Max Verstappen"),
        (2025, "Lando Norris"),
    };

    public static readonly (int Ano, string Nome)[] Equipes =
    {
        (1958, "Vanwall"),
        (1959, "Cooper-Climax"),
        (1960, "Cooper-Climax"),
        (1961, "Ferrari"),
        (1962, "BRM"),
        (1963, "Lotus-Climax"),
        (1964, "Ferrari"),
        (1965, "Lotus-Climax"),
        (1966, "Brabham-Repco"),
        (1967, "Brabham-Repco"),
        (1968, "Lotus-Ford"),
        (1969, "Matra-Ford"),
        (1970, "Team Lotus"),
        (1971, "Tyrrell"),
        (1972, "Team Lotus"),
        (1973, "Team Lotus"),
        (1974, "McLaren"),
        (1975, "Ferrari"),
        (1976, "Ferrari"),
        (1977, "Ferrari"),
        (1978, "Team Lotus"),
        (1979, "Ferrari"),
        (1980, "Williams"),
        (1981, "Williams"),
        (1982, "Ferrari"),
        (1983, "Ferrari"),
        (1984, "McLaren"),
        (1985, "McLaren"),
        (1986, "Williams"),
        (1987, "Williams"),
        (1988, "McLaren"),
        (1989, "McLaren"),
        (1990, "McLaren"),
        (1991, "McLaren"),
        (1992, "Williams"),
        (1993, "Williams"),
        (1994, "Williams"),
        (1995, "Benetton"),
        (1996, "Williams"),
        (1997, "Williams"),
        (1998, "McLaren"),
        (1999, "Ferrari"),
        (2000, "Ferrari"),
        (2001, "Ferrari"),
        (2002, "Ferrari"),
        (2003, "Ferrari"),
        (2004, "Ferrari"),
        (2005, "Renault"),
        (2006, "Renault"),
        (2007, "Ferrari"),
        (2008, "Ferrari"),
        (2009, "Brawn"),
        (2010, "Red Bull"),
        (2011, "Red Bull"),
        (2012, "Red Bull"),
        (2013, "Red Bull"),
        (2014, "Mercedes"),
        (2015, "Mercedes"),
        (2016, "Mercedes"),
        (2017, "Mercedes"),
        (2018, "Mercedes"),
        (2019, "Mercedes"),
        (2020, "Mercedes"),
        (2021, "Mercedes"),
        (2022, "Red Bull"),
        (2023, "Red Bull"),
        (2024, "McLaren"),
        (2025, "McLaren"),
    };

    /// <summary>Agrupa por campeão e ordena por quantidade de títulos (empates dividem a posição).</summary>
    public static IReadOnlyList<ItemRanking> Ranking((int Ano, string Nome)[] campeoes)
    {
        var lista = campeoes
            .GroupBy(c => c.Nome)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Max(c => c.Ano))
            .Select(g => new ItemRanking
            {
                Nome = g.Key,
                Titulos = g.Count(),
                Anos = string.Join(", ", g.Select(c => c.Ano).OrderBy(a => a))
            })
            .ToList();

        for (var i = 0; i < lista.Count; i++)
            lista[i].Posicao = i > 0 && lista[i].Titulos == lista[i - 1].Titulos ? lista[i - 1].Posicao : i + 1;

        return lista;
    }
}
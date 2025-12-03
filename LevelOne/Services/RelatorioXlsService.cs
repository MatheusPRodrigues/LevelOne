using ClosedXML.Excel;
using LevelOne.Models;

namespace LevelOne.Services;

public class RelatorioXlsService
{
    public byte[] GerarRelatorioEmXls(List<ChamadoModel> chamados)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Relatório de Chamados");

        ws.Cell("A1").Value = "Relatório de Chamados";
        ws.Range("A1:E1").Merge().Style
            .Font.SetBold()
            .Font.SetFontSize(18)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell("A2").Value = $"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}";
        ws.Range("A2:E2").Merge().Style
            .Font.SetItalic()
            .Font.SetFontColor(XLColor.Gray)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Row(3).Height = 5;

        string NormalizarStatus(string s)
            => s.Replace("_", "").Replace("-", "").Replace(" ", "").ToLower();

        var abertos = chamados.Where(c =>
            NormalizarStatus(c.StatusChamado.ToString()) == "aberto").ToList();

        var andamento = chamados.Where(c =>
            NormalizarStatus(c.StatusChamado.ToString()).Contains("ematendimento") ||
            NormalizarStatus(c.StatusChamado.ToString()).Contains("atendimento")
        ).ToList();

        var finalizados = chamados.Where(c =>
            NormalizarStatus(c.StatusChamado.ToString()).Contains("finalizado")).ToList();

        int totalChamados = chamados.Count;

        ws.Cell("A4").Value = "Resumo Geral";
        ws.Range("A4:E4").Merge().Style.Font.SetBold().Font.SetFontSize(14);

        string[] resumoHeaders = { "Total de Chamados", "Abertos", "Em Andamento", "Finalizados" };
        int[] resumoValues = { totalChamados, abertos.Count, andamento.Count, finalizados.Count };

        for (int i = 0; i < resumoHeaders.Length; i++)
        {
            ws.Cell(5, i + 1).Value = resumoHeaders[i];
            ws.Cell(6, i + 1).Value = resumoValues[i];

            ws.Cell(5, i + 1).Style.Font.SetBold();
            ws.Cell(5, i + 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
            ws.Cell(5, i + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell(6, i + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }

        int linha = 8;

        void CriarSecao(string titulo, List<ChamadoModel> lista, XLColor cor)
        {
            ws.Cell(linha, 1).Value = titulo;
            ws.Range(linha, 1, linha, 5).Merge().Style
                .Font.SetBold()
                .Font.SetFontSize(13)
                .Font.SetFontColor(cor);

            linha++;

            string[] cab = { "Título", "Cliente", "Técnico", "Status", "Data de Abertura" };

            for (int i = 0; i < cab.Length; i++)
            {
                ws.Cell(linha, i + 1).Value = cab[i];
                ws.Cell(linha, i + 1).Style.Font.SetBold();
                ws.Cell(linha, i + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(linha, i + 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
            }

            linha++;

            if (!lista.Any())
            {
                ws.Cell(linha, 1).Value = "Nenhum chamado nesta categoria.";
                ws.Range(linha, 1, linha, 5).Merge();
                linha += 2;
                return;
            }

            foreach (var c in lista)
            {
                ws.Cell(linha, 1).Value = c.Titulo;
                ws.Cell(linha, 2).Value = c.Cliente?.Nome ?? "-";
                ws.Cell(linha, 3).Value = c.Tecnico?.Nome ?? "-";
                ws.Cell(linha, 4).Value = c.StatusChamado.ToString();
                ws.Cell(linha, 5).Value = c.DataAbertura.ToString("dd/MM/yyyy");

                linha++;
            }

            linha += 2;
        }

        CriarSecao("Chamados Abertos", abertos, XLColor.Red);
        CriarSecao("Chamados em Andamento", andamento, XLColor.Orange);
        CriarSecao("Chamados Finalizados", finalizados, XLColor.Green);

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
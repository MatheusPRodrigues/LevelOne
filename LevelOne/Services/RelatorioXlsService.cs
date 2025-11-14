using ClosedXML.Excel;
using LevelOne.Models;

namespace LevelOne.Services;

public class RelatorioXlsService
{
    public byte[] GerarRelatorioEmXls(List<ChamadoModel> chamados)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Relatório de Chamados");

            worksheet.Cell("A1").Value = "Relatório de Chamados";
            worksheet.Range("A1:E1").Merge().Style
                .Font.SetBold()
                .Font.SetFontSize(18)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Cell("A2").Value = $"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range("A2:E2").Merge().Style
                .Font.SetItalic()
                .Font.SetFontColor(XLColor.Gray)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Row(3).Height = 5;

            var totalChamados = chamados.Count;
            var abertos = chamados.Where(c => c.StatusChamado.ToString().Equals("Aberto", StringComparison.OrdinalIgnoreCase)).ToList();
            var andamento = chamados.Where(c => c.StatusChamado.ToString().Contains("Andamento", StringComparison.OrdinalIgnoreCase)).ToList();
            var finalizados = chamados.Where(c => c.StatusChamado.ToString().Contains("Finalizado", StringComparison.OrdinalIgnoreCase)).ToList();

            worksheet.Cell("A4").Value = "Resumo Geral";
            worksheet.Cell("A4").Style.Font.SetBold().Font.SetFontSize(14);
            worksheet.Range("A4:E4").Merge();

            var resumoHeaders = new[] { "Total de Chamados", "Abertos", "Em Andamento", "Finalizados" };
            var resumoValues = new[] { totalChamados, abertos.Count, andamento.Count, finalizados.Count };

            for (int i = 0; i < resumoHeaders.Length; i++)
            {
                worksheet.Cell(5, i + 1).Value = resumoHeaders[i];
                worksheet.Cell(6, i + 1).Value = resumoValues[i];
                worksheet.Cell(5, i + 1).Style.Font.SetBold();
                worksheet.Cell(5, i + 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                worksheet.Cell(5, i + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(6, i + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }

            int linhaAtual = 8;

            void AdicionarSecao(string titulo, List<ChamadoModel> lista, XLColor cor)
            {
                worksheet.Cell(linhaAtual, 1).Value = titulo;
                worksheet.Cell(linhaAtual, 1).Style.Font.SetBold().Font.SetFontSize(13).Font.SetFontColor(cor);
                worksheet.Range(linhaAtual, 1, linhaAtual, 5).Merge();
                linhaAtual++;

                string[] cabecalho = { "Título", "Cliente", "Técnico", "Status", "Data de Abertura" };
                for (int i = 0; i < cabecalho.Length; i++)
                {
                    worksheet.Cell(linhaAtual, i + 1).Value = cabecalho[i];
                    worksheet.Cell(linhaAtual, i + 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    worksheet.Cell(linhaAtual, i + 1).Style.Font.SetBold();
                    worksheet.Cell(linhaAtual, i + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                }

                linhaAtual++;

                if (lista.Count == 0)
                {
                    worksheet.Cell(linhaAtual, 1).Value = "Nenhum chamado nesta categoria.";
                    worksheet.Range(linhaAtual, 1, linhaAtual, 5).Merge();
                    linhaAtual += 2;
                    return;
                }

                foreach (var c in lista)
                {
                    worksheet.Cell(linhaAtual, 1).Value = c.Titulo;
                    worksheet.Cell(linhaAtual, 2).Value = c.Cliente?.Nome ?? "-";
                    worksheet.Cell(linhaAtual, 3).Value = c.Tecnico?.Nome ?? "-";
                    worksheet.Cell(linhaAtual, 4).Value = c.StatusChamado.ToString();
                    worksheet.Cell(linhaAtual, 5).Value = c.DataAbertura.ToString("dd/MM/yyyy");

                    linhaAtual++;
                }

                linhaAtual += 2;
            }

            AdicionarSecao("Chamados Abertos", abertos, XLColor.Red);
            AdicionarSecao("Chamados em Andamento", andamento, XLColor.Orange);
            AdicionarSecao("Chamados Finalizados", finalizados, XLColor.Green);

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
}
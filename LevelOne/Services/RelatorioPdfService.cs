using System;
using System.Collections.Generic;
using System.Linq;
using LevelOne.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using LevelOne.Models;

namespace LevelOne.Services
{
    public class RelatorioPdfService
    {
        public RelatorioPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GerarRelatorioEmPdf(List<ChamadoModel> chamados)
        {
            var totalChamados = chamados.Count;
            var abertos = chamados.Where(c => c.StatusChamado == StatusEnum.Aberto).ToList();
            var andamento = chamados.Where(c => c.StatusChamado == StatusEnum.EmAtendimento).ToList();
            var finalizados = chamados.Where(c => c.StatusChamado == StatusEnum.Finalizado).ToList();

            var dataGeracao = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("RELATÓRIO DE CHAMADOS").FontSize(18).Bold().AlignCenter();
                        col.Item().Text($"Gerado em: {dataGeracao}").FontSize(9).FontColor(Colors.Grey.Darken1).AlignCenter();
                    });

                    page.Content().PaddingTop(10).Column(content =>
                    {
                        content.Item().Element(c => CriarResumo(c, totalChamados, abertos.Count, andamento.Count, finalizados.Count));

                        content.Item().PaddingTop(8).Element(CriarSumarioSimples());

                        if (abertos.Any())
                            content.Item().PaddingTop(10).Element(c => CriarSecao(c, "Chamados Abertos", abertos, Colors.Red.Medium));
                        if (andamento.Any())
                            content.Item().PaddingTop(10).Element(c => CriarSecao(c, "Chamados em Andamento", andamento, Colors.Orange.Medium));
                        if (finalizados.Any())
                            content.Item().PaddingTop(10).Element(c => CriarSecao(c, "Chamados Finalizados", finalizados, Colors.Green.Medium));
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void CriarResumo(IContainer container, int total, int abertos, int andamento, int finalizados)
        {
            container.Padding(6).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Total de Chamados").Bold();
                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Abertos").Bold();
                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Em Andamento").Bold();
                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Finalizados").Bold();

                table.Cell().Padding(6).Text(total.ToString()).FontSize(13).Bold();
                table.Cell().Padding(6).Text(abertos.ToString()).FontSize(13).Bold().FontColor(Colors.Red.Darken2);
                table.Cell().Padding(6).Text(andamento.ToString()).FontSize(13).Bold().FontColor(Colors.Orange.Darken2);
                table.Cell().Padding(6).Text(finalizados.ToString()).FontSize(13).Bold().FontColor(Colors.Green.Darken2);
            });
        }

        private Action<IContainer> CriarSumarioSimples()
        {
            return container =>
            {
                container.Stack(stack =>
                {
                    stack.Spacing(4);
                    stack.Item().Text("Sumário").FontSize(13).Bold().FontColor(Colors.Blue.Darken2);
                    stack.Item().Text("1. Chamados Abertos");
                    stack.Item().Text("2. Chamados em Andamento");
                    stack.Item().Text("3. Chamados Finalizados");
                });
            };
        }

        private void CriarSecao(IContainer container, string titulo, List<ChamadoModel> chamados, string cor)
        {
            container.Stack(stack =>
            {
                stack.Spacing(6);
                stack.Item().Text(titulo).FontSize(14).Bold().FontColor(cor);
                stack.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                stack.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); 
                        columns.RelativeColumn(2); 
                        columns.RelativeColumn(2); 
                        columns.RelativeColumn(2); 
                        columns.RelativeColumn(2); 
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Título").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Cliente").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Técnico").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Status").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Data Abertura").Bold();
                    });

                    foreach (var c in chamados)
                    {
                        table.Cell().Padding(6).Text(c.Titulo ?? "-");
                        table.Cell().Padding(6).Text(c.Cliente?.Nome ?? "-");
                        table.Cell().Padding(6).Text(c.Tecnico?.Nome ?? "-");
                        table.Cell().Padding(6).Text(c.StatusChamado.ToString());
                        table.Cell().Padding(6).Text(c.DataAbertura.ToString("dd/MM/yyyy"));
                    }
                });
            });
        }
    }
}

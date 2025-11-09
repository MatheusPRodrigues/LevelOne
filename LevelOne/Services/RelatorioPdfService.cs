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
            // Defina a licença (community) uma vez na inicialização da aplicação
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
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black));

                    // Cabeçalho
                    page.Header().Column(column =>
                    {
                        column.Item().Text("Relatório de Chamados").FontSize(20).Bold().FontColor(Colors.Blue.Medium).AlignCenter();
                        column.Item().Text($"Gerado em: {dataGeracao}").FontSize(9).FontColor(Colors.Grey.Darken1).AlignCenter();
                    });

                    // Conteúdo
                    page.Content()
                        .PaddingTop(10)
                        .Stack(stack =>
                        {
                            // Resumo geral
                            stack.Item().Element(ResumoGeral(totalChamados, abertos.Count, andamento.Count, finalizados.Count));

                            // Sumário (com links)
                            stack.Item().PaddingTop(10).Element(Sumario());

                            // Seções (cada uma é navegável por id)
                            if (abertos.Any())
                                stack.Item().PaddingTop(10).Element(Section("Abertos", "Chamados Abertos", abertos, Colors.Red.Medium));

                            if (andamento.Any())
                                stack.Item().PaddingTop(10).Element(Section("Andamento", "Chamados em Andamento", andamento, Colors.Orange.Medium));

                            if (finalizados.Any())
                                stack.Item().PaddingTop(10).Element(Section("Finalizados", "Chamados Finalizados", finalizados, Colors.Green.Medium));
                        });

                    // Rodapé
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

        // ---------- Helpers retornando Action<IContainer> (compatível com Element(...)) ----------

        private static Action<IContainer> ResumoGeral(int total, int abertos, int andamento, int finalizados)
        {
            return container =>
            {
                container.Padding(8).Border(1).BorderColor(Colors.Grey.Lighten3).CornerRadius(4).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    // Linha de cabeçalho
                    table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Total de Chamados").Bold();
                    table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Abertos").Bold();
                    table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Em Andamento").Bold();
                    table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Finalizados").Bold();

                    // Linha de valores (com destaque de cores)
                    table.Cell().Padding(6).Text(total.ToString()).FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                    table.Cell().Padding(6).Text(abertos.ToString()).FontSize(14).Bold().FontColor(Colors.Red.Darken2);
                    table.Cell().Padding(6).Text(andamento.ToString()).FontSize(14).Bold().FontColor(Colors.Orange.Darken2);
                    table.Cell().Padding(6).Text(finalizados.ToString()).FontSize(14).Bold().FontColor(Colors.Green.Darken2);
                });
            };
        }

        private static Action<IContainer> Sumario()
        {
            return container =>
            {
                container.PaddingTop(8).Column(col =>
                {
                    col.Item().Text("Sumário").FontSize(14).Bold().FontColor(Colors.Grey.Darken2);

                    col.Item().PaddingTop(6).Row(row =>
                    {
                        // Cada entrada usa Hyperlink + NavigateTo para saltar à Section
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("• ").FontSize(12);
                            text.Hyperlink("Chamados Abertos", "Abertos").FontSize(12).FontColor(Colors.Blue.Medium);
                        });

                        row.RelativeItem().Text(text =>
                        {
                            text.Span("• ").FontSize(12);
                            text.Hyperlink("Chamados em Andamento", "Andamento").FontSize(12).FontColor(Colors.Blue.Medium);
                        });

                        row.RelativeItem().Text(text =>
                        {
                            text.Span("• ").FontSize(12);
                            text.Hyperlink("Chamados Finalizados", "Finalizados").FontSize(12).FontColor(Colors.Blue.Medium);
                        });
                    });
                });
            };
        }

        private static Action<IContainer> Section(string id, string titulo, List<ChamadoModel> chamados, string cor)
        {
            return container =>
            {
                // Define o destino de navegação com Section(id)
                container.Section(id).PaddingVertical(6).Stack(stack =>
                {
                    stack.Item().Text(titulo).FontSize(16).Bold().FontColor(cor);
                    stack.Item().PaddingBottom(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // Tabela de chamados
                    stack.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Título
                            columns.RelativeColumn(3); // Cliente
                            columns.RelativeColumn(2); // Status
                            columns.RelativeColumn(2); // Data Abertura
                        });

                        // Cabeçalho
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Título").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Cliente").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Status").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Data Abertura").Bold();
                        });

                        // Linhas
                        foreach (var c in chamados)
                        {
                            table.Cell().Padding(6).Text(c.Titulo ?? "-");
                            table.Cell().Padding(6).Text(c.Cliente?.Nome ?? "-");
                            table.Cell().Padding(6).Text(c.StatusChamado.ToString());
                            table.Cell().Padding(6).Text(c.DataAbertura.ToString("dd/MM/yyyy"));
                        }
                    });
                });
            };
        }
    }
}

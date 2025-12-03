using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LevelOne.Helpers;

public static class CpfHelper
{
    public static bool ValidarCpf(string cpf, ModelStateDictionary modelState, string campo = "Cpf")
    {
        if (cpf.Length != 11)
        {
            modelState.AddModelError(campo, "CPF deve conter 11 dígitos.");
            return false;
        }

        if (cpf.All(c => c == cpf[0]))
        {
            modelState.AddModelError(campo, "CPF inválido.");
            return false;
        }

        if (!CpfEhValido(cpf))
        {
            modelState.AddModelError(campo, "CPF inválido.");
            return false;
        }

        return true;
    }

    private static bool CpfEhValido(string cpf)
    {
        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        int digito = resto < 2 ? 0 : 11 - resto;

        string cpfComDigito = tempCpf + digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(cpfComDigito[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return cpf.EndsWith($"{digito}{digito2}");
    }
}
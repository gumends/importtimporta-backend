namespace Domain;

public static class HTML
{
    public static string Gerar(string linkRedefinicao)
    {
        return $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Redefinição de Senha</title>
</head>
<body style='margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;'>

    <table width='100%' cellpadding='0' cellspacing='0'>
        <tr>
            <td align='center' style='padding:40px 16px;'>

                <table width='100%' max-width='600' cellpadding='0' cellspacing='0'
                       style='background-color:#ffffff; border-radius:8px; padding:32px;'>

                    <!-- LOGO -->
                    <tr>
                        <td align='center' style='padding-bottom:24px;'>
                            <img src='https://importtimporta.s3.us-east-1.amazonaws.com/logo-import.png'
                                 alt='Import Importa'
                                 style='max-width:200px; height:auto; display:block;'/>
                        </td>
                    </tr>

                    <!-- TÍTULO -->
                    <tr>
                        <td align='center' style='padding-bottom:24px;'>
                            <h2 style='margin:0; color:#333333;'>Redefinição de senha</h2>
                        </td>
                    </tr>

                    <!-- MENSAGEM -->
                    <tr>
                        <td style='color:#555555; font-size:16px; line-height:24px;'>
                            <p>Recebemos uma solicitação para redefinir a senha da sua conta.</p>
                            <p>Para continuar, clique no botão abaixo:</p>
                        </td>
                    </tr>

                    <!-- BOTÃO -->
                    <tr>
                        <td align='center' style='padding:32px 0;'>
                            <a href='{linkRedefinicao}'
                               style='background-color:#4f46e5;
                                      color:#ffffff;
                                      text-decoration:none;
                                      padding:14px 28px;
                                      border-radius:6px;
                                      font-size:16px;
                                      font-weight:bold;
                                      display:inline-block;'>
                                Redefinir senha
                            </a>
                        </td>
                    </tr>

                    <!-- LINK ALTERNATIVO -->
                    <tr>
                        <td style='color:#555555; font-size:14px; line-height:22px;'>
                            <p>Se o botão não funcionar, copie e cole o link abaixo no seu navegador:</p>
                            <p style='word-break:break-all; color:#4f46e5;'>
                                {linkRedefinicao}
                            </p>
                        </td>
                    </tr>

                    <!-- RODAPÉ -->
                    <tr>
                        <td style='padding-top:24px; font-size:12px; color:#888888;'>
                            <p>
                                Se você não solicitou a redefinição de senha, pode ignorar este e-mail com segurança.
                            </p>
                        </td>
                    </tr>

                </table>

                <p style='margin-top:16px; font-size:12px; color:#999999;'>
                    © {DateTime.Now.Year} Importt Importa. Todos os direitos reservados.
                </p>

            </td>
        </tr>
    </table>

</body>
</html>";
    }
}
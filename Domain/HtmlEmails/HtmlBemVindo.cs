namespace Domain;

public class HtmlBemVindo
{
public static string Gerar(string nome)
{
    return $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Boas-vindas</title>
</head>
<body style='margin:0;padding:0;font-family:Arial,Helvetica,sans-serif;background-color:#f4f6f8;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f6f8;padding:20px 0;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' 
                       style='background:#ffffff;border-radius:8px;padding:40px 30px;box-shadow:0 4px 12px rgba(0,0,0,0.08);'>
                    
                    <tr>
                        <td align='center' style='padding-bottom:20px;'>
                            <h1 style='margin:0;color:#2c3e50;font-size:24px;'>
                                Seja bem-vindo(a), {nome}!
                            </h1>
                        </td>
                    </tr>

                    <tr>
                        <td style='color:#555;font-size:16px;line-height:1.6;'>
                            <p style='margin:0 0 16px 0;'>
                                É um prazer ter você conosco 🎉
                            </p>

                            <p style='margin:0 0 16px 0;'>
                                Sua conta foi criada com sucesso e agora você já pode aproveitar todos os recursos disponíveis.
                            </p>

                            <p style='margin:0;'>
                                Se tiver qualquer dúvida, nossa equipe está pronta para ajudar.
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td align='center' style='padding-top:30px;'>
                            <a href='#' 
                               style='background-color:#007bff;color:#ffffff;text-decoration:none;
                                      padding:12px 24px;border-radius:6px;font-weight:bold;
                                      display:inline-block;'>
                                Acessar minha conta
                            </a>
                        </td>
                    </tr>

                    <tr>
                        <td align='center' style='padding-top:30px;color:#999;font-size:12px;'>
                            © {DateTime.Now.Year} Sua Empresa. Todos os direitos reservados.
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
}
}
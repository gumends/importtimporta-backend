# Cadastra a linha iPhone 18 na loja (tipo 1) e insere imagens.
$ErrorActionPreference = "Stop"
$api = "http://localhost:5083/produto"

$imgProMax = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-16-pro-max-naturaltitanium-select?wid=400&hei=400&fmt=png-alpha"
$imgPro = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-16-pro-naturaltitanium-select?wid=400&hei=400&fmt=png-alpha"
$imgBase = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-16-ultramarine-select?wid=400&hei=400&fmt=png-alpha"
$imgAir = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-16-plus-ultramarine-select?wid=400&hei=400&fmt=png-alpha"

function New-ProdutoLoja {
  param($Payload)
  $json = $Payload | ConvertTo-Json -Depth 8 -Compress
  Invoke-RestMethod -Method POST -Uri $api -ContentType "application/json; charset=utf-8" -Body $json
}

function Add-ImagemSql {
  param([guid]$ProdutoId, [string]$Caminho, [string]$Descricao)
  $imgId = [guid]::NewGuid()
  $c = $Caminho.Replace("'", "''")
  $d = $Descricao.Replace("'", "''")
  $sql = @"
INSERT INTO imagens (Id, Caminho, Descricao, ProdutoId)
VALUES ('$imgId', N'$c', N'$d', '$ProdutoId');
"@
  sqlcmd -C -S "localhost\SQLEXPRESS" -d importtimporta -E -Q $sql | Out-Null
}

$catalogo = @(
  @{
    key = "proMax"
    body = @{
      nomeProduto = "iPhone 18 Pro Max 256GB"
      valorOriginal = 8000
      desconto = 200
      valorParcelado = 666.67
      descricao = "Tela imersiva, chip A19 Pro e bateria pensada para o dia inteiro."
      tipoProduto = 1
      disponivel = $true
      mesesGarantia = 12
      quantidade = 12
      color = "#32374A"
      colorName = "Preto espacial"
      informacoesProduto = @{
        marca = "Apple"
        armazenamentoInterno = "256 GB"
        tipoTela = "Super Retina XDR"
        tamanhoTela = "6,9 pol."
        processador = "A19 Pro"
        sistemaOperacional = "iOS 18"
        material = "Titanio"
      }
    }
    imagem = $imgProMax
  },
  @{
    key = "pro"
    body = @{
      nomeProduto = "iPhone 18 Pro 256GB"
      valorOriginal = 12999
      desconto = 0
      valorParcelado = 1083.25
      descricao = "Potencia Pro em um design refinado, feito para criar, jogar e produzir."
      tipoProduto = 1
      disponivel = $true
      mesesGarantia = 12
      quantidade = 10
      color = "#F77E2D"
      colorName = "Laranja cosmico"
      informacoesProduto = @{
        marca = "Apple"
        armazenamentoInterno = "256 GB"
        tipoTela = "Super Retina XDR"
        tamanhoTela = "6,3 pol."
        processador = "A19 Pro"
        sistemaOperacional = "iOS 18"
        material = "Titanio"
      }
    }
    imagem = $imgPro
  },
  @{
    key = "base"
    body = @{
      nomeProduto = "iPhone 18 128GB"
      valorOriginal = 6999
      desconto = 0
      valorParcelado = 583.25
      descricao = "Cameras inteligentes e cores vibrantes no iPhone mais versatil da linha."
      tipoProduto = 1
      disponivel = $true
      mesesGarantia = 12
      quantidade = 15
      color = "#96AED1"
      colorName = "Azul"
      informacoesProduto = @{
        marca = "Apple"
        armazenamentoInterno = "128 GB"
        tipoTela = "Super Retina XDR"
        tamanhoTela = "6,1 pol."
        processador = "A19"
        sistemaOperacional = "iOS 18"
        material = "Aluminio"
      }
    }
    imagem = $imgBase
  },
  @{
    key = "air"
    body = @{
      nomeProduto = "iPhone Air 256GB"
      valorOriginal = 8999
      desconto = 0
      valorParcelado = 749.92
      descricao = "Ultrafino, leve e elegante. Tecnologia premium no bolso."
      tipoProduto = 1
      disponivel = $true
      mesesGarantia = 12
      quantidade = 8
      color = "#F0F9FF"
      colorName = "Azul claro"
      informacoesProduto = @{
        marca = "Apple"
        armazenamentoInterno = "256 GB"
        tipoTela = "Super Retina XDR"
        tamanhoTela = "6,6 pol."
        processador = "A19"
        sistemaOperacional = "iOS 18"
        material = "Aluminio"
      }
    }
    imagem = $imgAir
  }
)

$ids = @{}
foreach ($item in $catalogo) {
  $created = New-ProdutoLoja -Payload $item.body
  $id = [guid]$created.id
  $ids[$item.key] = $id.ToString()
  Add-ImagemSql -ProdutoId $id -Caminho $item.imagem -Descricao "Imagem principal"
  Write-Host "$($item.key): $($ids[$item.key])"
}

Write-Host ""
Write-Host "Cole estes IDs no ProtudoAtual.json:"
Write-Host "  Pro Max (id 7): $($ids['proMax'])"
Write-Host "  Pro (id 8):     $($ids['pro'])"
Write-Host "  iPhone 18 (9):  $($ids['base'])"
Write-Host "  Air (id 10):    $($ids['air'])"

$ids | ConvertTo-Json | Set-Content -Path "$PSScriptRoot\iphone18-produto-ids.json" -Encoding UTF8

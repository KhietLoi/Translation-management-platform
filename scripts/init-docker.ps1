$ErrorActionPreference = 'Stop'

# Tìm root Platform từ vị trí thư mục scripts.
$root = Split-Path $PSScriptRoot -Parent

# Tạo nơi chứa cấu hình Docker local.
New-Item -ItemType Directory -Force `
    (Join-Path $root 'infra/docker') | Out-Null

# Mỗi phần tử gồm đường dẫn API và tên file cấu hình đầu ra.
$services = @(
    @('Management/MySolution.Api', 'api'),
    @('Email/MySolution.Email.Api', 'email')
)

foreach ($item in $services) {
    $destination = Join-Path $root `
        "infra/docker/$($item[1]).local.json"

    # Giữ cấu hình local nếu người dùng đã tạo hoặc chỉnh sửa.
    if (Test-Path $destination) {
        continue
    }

    $sourcePath = Join-Path $root `
        "services/backend/$($item[0])/appsettings.json"

    $config = Get-Content $sourcePath -Raw | ConvertFrom-Json

    # Container dùng host.docker.internal để truy cập máy Windows.
    if ($config.ConnectionStrings) {
        foreach ($property in $config.ConnectionStrings.PSObject.Properties) {
            $property.Value = $property.Value -replace `
                '(?i)(Host|Server)\s*=\s*(localhost|127\.0\.0\.1)(?=;|$)', `
                '$1=host.docker.internal'
        }
    }

    # Điều chỉnh địa chỉ AI nếu AI đang chạy local trên Windows.
    if ($config.AI.BaseUrl) {
        $config.AI.BaseUrl = $config.AI.BaseUrl -replace `
            '://(localhost|127\.0\.0\.1)(?=[:/]|$)', `
            '://host.docker.internal'
    }

    $config |
        ConvertTo-Json -Depth 30 |
        Set-Content $destination -Encoding utf8
}

# Sinh mật khẩu RabbitMQ nếu chưa có.
$envPath = Join-Path $root '.env'

if (
    !(Test-Path $envPath) -or
    !(Select-String -LiteralPath $envPath `
        -Pattern '^RABBITMQ_PASSWORD=' -Quiet)
) {
    Add-Content $envPath `
        "`nRABBITMQ_PASSWORD=$([Guid]::NewGuid().ToString('N'))" `
        -Encoding utf8
}

Write-Output 'Docker local configuration is ready.'
@echo off
chcp 65001 >nul
setlocal EnableDelayedExpansion

:: ============================================================
:: tag-release.bat — Cria e publica uma tag de versão no Git.
:: Uso: tag-release.bat <versão> [mensagem]
::
:: Exemplos:
::   tag-release.bat 1.2.0
::   tag-release.bat 1.2.0-beta.1
::   tag-release.bat 1.2.0 "Lançamento da versão 1.2.0"
:: ============================================================

if "%~1"=="" (
    echo.
    echo  ERRO: informe a versão como primeiro argumento.
    echo  Uso:  tag-release.bat ^<versão^> [mensagem]
    echo  Ex.:  tag-release.bat 1.2.0
    echo        tag-release.bat 1.2.0-beta.1 "Descricao opcional"
    echo.
    exit /b 1
)

set "VERSION=%~1"
set "TAG=v%VERSION%"
set "MESSAGE=%~2"
if "!MESSAGE!"=="" set "MESSAGE=Versão %VERSION%"

:: Valida formato semver básico (MAJOR.MINOR.PATCH com pré-release opcional)
echo %VERSION% | findstr /R "^[0-9][0-9]*\.[0-9][0-9]*\.[0-9][0-9]*" >nul 2>&1
if errorlevel 1 (
    echo.
    echo  ERRO: versão inválida — use o formato MAJOR.MINOR.PATCH, ex: 1.2.0 ou 1.2.0-beta.1
    echo.
    exit /b 1
)

:: Verifica se estamos dentro de um repositório Git
git rev-parse --git-dir >nul 2>&1
if errorlevel 1 (
    echo.
    echo  ERRO: diretório atual não é um repositório Git.
    echo.
    exit /b 1
)

:: Verifica se a tag já existe localmente
git tag --list "%TAG%" | findstr /I "%TAG%" >nul 2>&1
if not errorlevel 1 (
    echo.
    echo  ERRO: a tag %TAG% já existe localmente.
    echo  Para removê-la:  git tag -d %TAG%
    echo.
    exit /b 1
)

:: Garante que estamos na branch main
for /f "delims=" %%B in ('git rev-parse --abbrev-ref HEAD') do set "BRANCH=%%B"
if /i "!BRANCH!" neq "main" (
    echo.
    echo  ERRO: tags de release só podem ser criadas a partir da branch main.
    echo  Branch atual: !BRANCH!
    echo  Execute: git checkout main
    echo.
    exit /b 1
)

:: Sincroniza com o remoto antes de tagear para garantir que HEAD está no histórico de main
echo.
echo  Sincronizando com origin/main...
git fetch origin main >nul 2>&1
if errorlevel 1 (
    echo.
    echo  ERRO: falha ao executar git fetch. Verifique a conexão com o remoto.
    echo.
    exit /b 1
)

:: Compara HEAD local com origin/main — qualquer divergência aborta
for /f "delims=" %%L in ('git rev-parse HEAD') do set "LOCAL_SHA=%%L"
for /f "delims=" %%R in ('git rev-parse origin/main') do set "REMOTE_SHA=%%R"
if "!LOCAL_SHA!" neq "!REMOTE_SHA!" (
    echo.
    echo  ERRO: o HEAD local não está sincronizado com origin/main.
    echo  Local : !LOCAL_SHA!
    echo  Remote: !REMOTE_SHA!
    echo  Execute "git pull origin main" e tente novamente.
    echo.
    exit /b 1
)

:: Executa os testes de integração locais antes de criar a tag
:: Os testes são pulados automaticamente quando os User Secrets não estão configurados.
echo.
echo  Executando testes de integração locais...
dotnet test integration-tests\Nfe.Paulistana.IntegrationTests.csproj -c Release --nologo -v minimal
if errorlevel 1 (
    echo.
    echo  ERRO: testes de integração falharam. Corrija os erros antes de publicar a nova versão.
    echo.
    exit /b 1
)

:: Exibe o estado atual do repositório
echo.
echo  Repositório : %CD%
echo  Tag         : %TAG%
echo  Mensagem    : !MESSAGE!
echo  Commit HEAD : 
git log -1 --oneline
echo.

:: Confirmação antes de prosseguir
set /p "CONFIRM= Confirma a publicação da tag %TAG%? [s/N] "
if /i "!CONFIRM!" neq "s" (
    echo  Operação cancelada.
    echo.
    exit /b 0
)

:: Cria a tag anotada localmente
echo.
echo  [1/2] Criando tag %TAG% localmente...
git tag -a "%TAG%" -m "!MESSAGE!"
if errorlevel 1 (
    echo  ERRO: falha ao criar a tag local.
    exit /b 1
)

:: Publica a tag no remoto (origin)
echo  [2/2] Publicando %TAG% no remoto ^(origin^)...
git push origin "%TAG%"
if errorlevel 1 (
    echo.
    echo  ERRO: falha ao publicar a tag no remoto.
    echo  A tag foi criada localmente. Para tentar novamente: git push origin %TAG%
    echo  Para desfazer a tag local:                          git tag -d %TAG%
    echo.
    exit /b 1
)

echo.
echo  Tag %TAG% publicada com sucesso!
echo.
endlocal

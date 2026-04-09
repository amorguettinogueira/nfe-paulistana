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

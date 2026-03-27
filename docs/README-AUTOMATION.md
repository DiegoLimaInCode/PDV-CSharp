# Sistema de Atualização Automática do README

## 📖 Visão Geral

Este repositório inclui um sistema automatizado que mantém o README.md sempre atualizado com os commits mais recentes. Toda vez que há um push para as branches `main` ou `master`, o GitHub Actions executa um workflow que atualiza a seção de histórico de commits.

## 🔧 Como Funciona

### 1. Workflow do GitHub Actions

O arquivo `.github/workflows/update-readme.yml` define um workflow que:

- **Dispara automaticamente** quando há um push nas branches `main` ou `master`
- **Ignora mudanças** no próprio README.md para evitar loops infinitos
- **Busca os 10 commits mais recentes** usando `git log`
- **Atualiza a seção de commits** no README.md
- **Faz commit e push** das mudanças automaticamente

### 2. Marcadores no README

O README.md contém marcadores especiais que delimitam a seção de commits:

```markdown
<!-- COMMITS_START -->
[Conteúdo atualizado automaticamente]
<!-- COMMITS_END -->
```

O script Python procura por esses marcadores e substitui o conteúdo entre eles com a tabela de commits mais recente.

### 3. Script Python

Um script Python inline é executado pelo workflow para:

- Executar `git log` e formatar os commits
- Ler o README.md atual
- Localizar os marcadores `<!-- COMMITS_START -->` e `<!-- COMMITS_END -->`
- Substituir o conteúdo entre os marcadores
- Salvar o README atualizado

## 📋 Formato da Tabela de Commits

A tabela gerada inclui:

| Coluna | Descrição |
|--------|-----------|
| **Hash** | Hash curto do commit (7 caracteres) |
| **Data** | Data e hora do commit (DD/MM/YYYY HH:MM) |
| **Mensagem** | Mensagem do commit |
| **Autor** | Nome do autor do commit |

## 🔒 Segurança

- O workflow tem permissão `contents: write` apenas para atualizar o README
- O commit automático usa a identidade `github-actions[bot]`
- A diretiva `[skip ci]` evita que o próprio commit de atualização dispare o workflow novamente
- O `paths-ignore` adiciona uma camada extra de proteção contra loops infinitos

## ✨ Personalização

### Alterar o número de commits exibidos

Para mudar de 10 para outro número de commits, edite a linha no workflow:

```python
['git', 'log', '--pretty=format:| %h | %ad | %s | %an |', '--date=format:%d/%m/%Y %H:%M', '-10'],
```

Substitua `-10` pelo número desejado (ex: `-20` para 20 commits).

### Alterar o formato da data

Para mudar o formato da data, modifique a parte `--date=format:%d/%m/%Y %H:%M`:

- `%d/%m/%Y` - Dia/Mês/Ano
- `%H:%M` - Hora:Minuto
- Exemplo para formato americano: `%m/%d/%Y %I:%M %p`

### Adicionar mais colunas

Para adicionar informações adicionais à tabela (como email, hash completo, etc.), modifique o formato em:

```python
'--pretty=format:| %h | %ad | %s | %an |'
```

Placeholders disponíveis:
- `%h` - Hash abreviado
- `%H` - Hash completo
- `%an` - Nome do autor
- `%ae` - Email do autor
- `%s` - Mensagem do commit
- `%ad` - Data do autor
- `%cn` - Nome do committer

## 🚀 Manutenção

### O workflow não está funcionando?

1. Verifique se o workflow tem permissões adequadas em **Settings → Actions → General**
2. Confirme que os marcadores `<!-- COMMITS_START -->` e `<!-- COMMITS_END -->` existem no README.md
3. Verifique os logs do workflow em **Actions** no GitHub

### Desabilitar temporariamente

Para desabilitar o workflow temporariamente, adicione ao início do arquivo `.github/workflows/update-readme.yml`:

```yaml
on:
  workflow_dispatch:  # Apenas manual
```

## 📝 Notas

- O workflow é executado **após cada push**, não em pull requests
- As atualizações do README são commitadas automaticamente pelo bot do GitHub Actions
- O histórico de commits é sempre baseado na branch onde o push foi feito

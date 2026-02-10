# 🛒 PDV-CSharp (Point of Sale System)

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)

Um sistema de Ponto de Venda (PDV) moderno desenvolvido em C# com WPF para Windows.

## 📋 Sobre o Projeto

PDV-CSharp é um sistema completo de ponto de venda desenvolvido com tecnologias .NET modernas, oferecendo uma interface intuitiva e recursos robustos para gerenciamento de vendas em estabelecimentos comerciais.

## 🏗️ Estrutura do Projeto

O projeto está organizado em três camadas principais:

```
PDV-CSharp/
├── PDVCSharp.WPF/          # Camada de apresentação (Interface do usuário)
│   ├── Sections/           # Telas da aplicação
│   │   ├── Login.xaml      # Tela de login
│   │   ├── Venda.xaml      # Tela de vendas
│   │   └── Abertura.xaml   # Tela de abertura de caixa
│   └── MainWindow.xaml     # Janela principal
├── PDVCSharp.Domain/       # Camada de domínio (Regras de negócio)
└── PDVCSharp.Data/         # Camada de dados (Acesso a dados)
```

## 🚀 Tecnologias Utilizadas

- **.NET 10.0** - Framework de desenvolvimento
- **C# 12.0** - Linguagem de programação
- **WPF (Windows Presentation Foundation)** - Framework para interface gráfica
- **XAML** - Linguagem de marcação para UI

## 📦 Funcionalidades

- 🔐 **Sistema de Login** - Autenticação de usuários
- 💰 **Gerenciamento de Vendas** - Processamento de transações
- 📊 **Abertura de Caixa** - Controle de fluxo de caixa
- 🖥️ **Interface Moderna** - Design clean e intuitivo

## 💻 Requisitos

- Windows 10 ou superior
- .NET 10.0 SDK ou superior
- Visual Studio 2022 ou superior (recomendado)

## 🔧 Instalação

1. Clone o repositório:
```bash
git clone https://github.com/DiegoLimaInCode/PDV-CSharp.git
```

2. Abra o arquivo `PDVCSharp.slnx` no Visual Studio

3. Restaure os pacotes NuGet:
```bash
dotnet restore
```

4. Compile o projeto:
```bash
dotnet build
```

5. Execute o projeto:
```bash
dotnet run --project PDVCSharp.WPF
```

## 🎯 Como Usar

1. Execute a aplicação
2. Faça login com suas credenciais
3. Abra o caixa para iniciar as operações
4. Realize vendas através da interface de vendas

## 📝 Histórico de Commits Recentes

<!-- COMMITS_START -->
<!-- Este conteúdo é atualizado automaticamente -->

### 📊 Últimos 10 Commits

| Hash | Data | Mensagem | Autor |
|------|------|----------|-------|
| 0ed9278 | 10/02/2026 14:33 | Initial plan | copilot-swe-agent[bot] |
| 76c7e7a | 10/02/2026 11:20 | Add .gitignore and remove tracked files that should be ignored | diego |

<!-- COMMITS_END -->

## 👨‍💻 Desenvolvedor

**Diego Lima**

- GitHub: [@DiegoLimaInCode](https://github.com/DiegoLimaInCode)

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

## 🤝 Contribuições

Contribuições são bem-vindas! Sinta-se à vontade para:

1. Fazer um Fork do projeto
2. Criar uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abrir um Pull Request

---

<div align="center">
  Feito com ❤️ por Diego Lima
</div>

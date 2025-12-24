/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.util.*;
import java.io.*;

/**
 *
 * @author Diogo
 */
public class MenuPrincipal implements IMenu{
    private IMapa mapa;
    private Scanner scanner;
    private Turno turno;
    private Civilizacao civilizacao;
    private ArrayList<Civilizacao> civilizacoes;
    private GestorMovimento movimento;
    private GestorAtaque ataque;
    private GestorConstrucao construcao;

    public MenuPrincipal(IMapa mapa, Civilizacao civilizacao, 
            ArrayList<Civilizacao> civilizacoes) {
        this.mapa = mapa;
        this.scanner = new Scanner(System.in);
        this.turno = new Turno(mapa);
        this.civilizacao = civilizacao;
        this.civilizacoes = civilizacoes;
        this.movimento = new GestorMovimento(mapa); // Única instância
        this.ataque = new GestorAtaque(mapa);
        this.construcao = new GestorConstrucao(mapa);
        
    }
    /**
     * Exibe as opções disponíveis no menu da civilização.
     *
     * Este método apresenta um menu com várias opções, incluindo a visualização 
     * de cidades, o mapa, iniciar um novo turno, informações sobre a civilização, 
     * e opções para gravar ou carregar o jogo.
     */
    @Override
    public void exibirOpcoes() {
        GestorMensagens.mostrarMensagem("\nCivilizacao");
        GestorMensagens.mostrarMensagem("1. Cidades");
        GestorMensagens.mostrarMensagem("2. Ver o mapa");
        GestorMensagens.mostrarMensagem("3. Novo turno");
        GestorMensagens.mostrarMensagem("4. Informacao");
        GestorMensagens.mostrarMensagem("5. Gravar");
        GestorMensagens.mostrarMensagem("6. Carregar");
        GestorMensagens.mostrarMensagem("0. Sair");
        GestorMensagens.mostrarMensagem("Escolha uma opcao no Menu Principal: ");
    }

    /**
     * Processa a escolha do jogador no menu da civilização.
     *
     * Este método exibe as opções disponíveis e executa a ação correspondente 
     * com base na escolha do jogador. O loop continua até que o jogador decida 
     * sair do jogo.
     *
     * @param opcao a opção escolhida pelo jogador
     * @throws IOException se ocorrer um erro de entrada/saída durante a gravação ou carregamento
     */
    @Override
    public void processarEscolha(int opcao) throws IOException {   
        boolean continuar = true;
        while (continuar) {
            exibirOpcoes();
            try {
                opcao = scanner.nextInt();
                switch (opcao) { 
                    case 1:
                        opcaoCidades();
                        break;    
                    case 2:
                        GestorMensagens.mostrarMensagem("\nExibindo o mapa:");
                        mapa.mostrarMapa(); // Mostra o mapa atual 
                        break;
                    case 3:
                        turno.iniciarTurno(civilizacao, movimento, ataque, construcao); 
                        break;  
                    case 4:
                        GestorMensagens.mostrarMensagem("\n******Info******");
                        informacaoCivilizacoes();
                        break;
                    case 5:
                        gravarJogo();
                        break;
                    case 6:
                        carregarJogo();
                        GestorMensagens.mostrarMensagem("Jogo carregado com sucesso!");
                        break;    
                    case 0:
                        GestorMensagens.mostrarMensagem("\nSaindo do jogo.");
                        continuar = false;
                        break;
                    default:
                        GestorMensagens.mostrarMensagem("\nOpcao invalida. Tente novamente.");
                        break;
                }
            } catch (InputMismatchException e) {
                GestorMensagens.mostrarMensagem("Entrada invalida! Insira um numero.");
                scanner.nextLine(); // Limpa o buffer de entrada inválida
            }
        }
    }

    /**
     * Grava o estado atual do jogo em arquivos.
     *
     * Este método salva o número do turno, o estado do mapa, a civilização 
     * atual e as civilizações existentes em arquivos separados. Se ocorrer 
     * um erro durante a gravação, uma mensagem de erro é exibida.
     *
     * @throws IOException se ocorrer um erro de entrada/saída durante a gravação
     */
    private void gravarJogo() throws IOException {
        try {
            BufferedWriter writer = new BufferedWriter(new FileWriter("saveMapa.txt"));
            writer.write(String.valueOf(Turno.getNumTurno())); // Conversão para String
            writer.newLine();
            ((IEstadoJogo) mapa).gravaDados(writer);
            writer.close();

            writer = new BufferedWriter(new FileWriter("saveCivilizacao.txt"));
            civilizacao.gravaDados(writer);
            writer.close();

            writer = new BufferedWriter(new FileWriter("saveCivilizacoes.txt"));
            for (Civilizacao civilizacao : civilizacoes) {
                civilizacao.gravaDados(writer);
            }
            writer.close();

            GestorMovimento.gravarMovimentos("saveMovimentos.txt");
            GestorConstrucao.gravarConstrucoes("saveConstrucoes.txt");
        } catch (IOException e) {
            System.err.println("Erro ao gravar o jogo: " + e.getMessage());
            throw e; // Relança a exceção para informar o erro
        }
        GestorMensagens.mostrarMensagem("Jogo gravado com sucesso!");
    }

    /**
     * Carrega o estado do jogo a partir de arquivos.
     *
     * Este método lê os dados do jogo a partir de arquivos salvos, incluindo 
     * o estado do mapa, a civilização atual e as civilizações existentes. 
     * Se ocorrer um erro durante o carregamento, uma mensagem de erro é exibida.
     *
     * @throws IOException se ocorrer um erro de entrada/saída durante o carregamento
     */
    private void carregarJogo() throws IOException {
        try {
            BufferedReader reader = new BufferedReader(new FileReader("saveMapa.txt"));
            Turno.setNumTurno(Integer.parseInt(reader.readLine())); // Carrega o turno
            IMapa novoMapa = (IMapa) ((IEstadoJogo) mapa).carregarEstado(reader, null, false);
            this.mapa = novoMapa; // Atualiza o mapa da classe   
            reader.close();
            GestorMovimento.getMovimentosPlaneados().clear(); // Limpa a lista antes de carregar novos dados
            GestorConstrucao.getConstrucoesPlaneadas().clear(); // Limpa a lista antes de carregar novos dados
            reader = new BufferedReader(new FileReader("saveCivilizacao.txt"));
            Civilizacao civilizacao = this.civilizacao.carregarEstado(reader, mapa, true);
            this.civilizacao = civilizacao;
            reader.close();
            reader = new BufferedReader(new FileReader("saveCivilizacoes.txt"));            
            ArrayList<Civilizacao> civilizacoes = new ArrayList<>();
            for (Civilizacao civi : this.civilizacoes) {
                Civilizacao novaCivi = civi.carregarEstado(reader, mapa, false);
                civilizacoes.add(novaCivi);
            }
            reader.close();
            this.civilizacoes = civilizacoes;          
        } catch (IOException e) {
            System.err.println("Erro ao carregar o jogo: " + e.getMessage());
            throw e; // Relança a exceção para informar o erro
        }    
    }
    /**
     * Exibe informações sobre a civilização atual e todas as civilizações 
     * disponíveis.
     *
     * Este método mostra os detalhes da civilização atual e, em seguida, 
     * itera sobre a lista de civilizações, exibindo informações sobre cada uma.
     */
    private void informacaoCivilizacoes() {
        GestorMensagens.mostrarMensagem(civilizacao);
        for (Civilizacao civi : civilizacoes) {
            GestorMensagens.mostrarMensagem("\n" + civi);
        }
    }

    /**
     * Exibe as cidades disponíveis e permite que o jogador escolha uma cidade.
     *
     * Este método lista todas as cidades no mapa e solicita ao jogador que 
     * escolha uma delas. Se a escolha for válida, o menu da cidade correspondente 
     * é exibido; caso contrário, uma mensagem de erro é mostrada.
     */
    private void opcaoCidades() {
        exibeCidades();
        GestorMensagens.mostrarMensagem("\nEscolha uma cidade: ");
        int cidadeIndex = scanner.nextInt() - 1;

        if (cidadeIndex >= 0 && cidadeIndex < mapa.getCidades().size()) {
            Cidade cidade = mapa.getCidades().get(cidadeIndex);
            menuCidade(cidade);
        } else {
            GestorMensagens.mostrarMensagem("\nCidade invalida.");
        }
    }

    /**
     * Cria e exibe o menu da cidade, permitindo interações com a cidade 
     * selecionada.
     *
     * Este método inicializa o menu da cidade e processa as escolhas do jogador. 
     * Dependendo se a cidade pertence à civilização do jogador ou é uma cidade 
     * inimiga, o menu apropriado é exibido.
     *
     * @param cidade a cidade para a qual o menu será exibido
     */
    private void menuCidade(Cidade cidade) {
        // Cria o menu cidade e entra no loop de interação
        MenuCidade menuCidade = new MenuCidade(cidade, mapa, movimento, ataque, construcao);
        int opcaoCidade = -1;

        if (civilizacao.getCidades().contains(cidade)) {
            menuCidade.processarEscolha(opcaoCidade);
        } else {
            menuCidade.processarEscolhaInimigas(opcaoCidade, cidade.getNome());
        }
    }

    /**
     * Exibe a lista de cidades disponíveis no mapa.
     *
     * Este método lista todas as cidades no mapa, mostrando o nome de cada 
     * cidade e a civilização a que pertence. As cidades são exibidas numericamente 
     * para facilitar a seleção.
     */
    private void exibeCidades() {
        GestorMensagens.mostrarMensagem("\nCIDADES:");
        for (int i = 0; i < mapa.getCidades().size(); i++) {
            GestorMensagens.mostrarMensagem((i + 1) + ". " + 
                    mapa.getCidades().get(i).getNome() + " -> " + 
                    mapa.getCidades().get(i).getCivilizacao().getNome()); // Exibe cada cidade numericamente
        }
    }
}

/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileReader;
import java.io.IOException;
import java.util.*;

/**
 * Classe que representa uma cidade com informações sobre o nome, civilização, 
 * população e área. Fornece métodos para calcular a densidade populacional 
 * e gerenciar o estado e as ações da cidade.
 */
public class Cidade extends TerrenoCidade implements IAtacavel{
    private static int distMaxAloc; // Distância máxima para alocação de cidadãos
    private static int distMinCidades; // Distância mínima padrão entre cidades   
    private int posX, posY, trabalhadores, defesa, forcaCombate;
    private String nome;    
    private Populacao populacao;
    private ArrayList<ITerreno> terrenosTrabalhados; // Células atribuídas para trabalho
    private IMapa mapa; // Referência ao mapa principal
    private GestorRecursos recursos;
    private GestorUnidades unidades;
    private Civilizacao civilizacao;
    private Random rand;
    /**
     * Construtor para inicializar os atributos da classe Cidade.
     *
     * @param nome Nome da cidade.
     * @param posX Coordenada X da cidade.
     * @param posY Coordenada Y da cidade.
     * @param mapa Mapa associado à cidade.
     * @param civilizacao Civilização à qual a cidade pertence.
     */
    public Cidade(String nome, int posX, int posY, IMapa mapa, Civilizacao civilizacao) {
        super(posX, posY);       
        rand = new Random();
        this.nome = nome;
        this.posX = posX;
        this.posY = posY;
        this.populacao = new Populacao(this, (rand.nextInt(3001) + 2000)); // População entre 3,000 e 5,000);
        this.defesa = 200;
        this.forcaCombate = 20;
        this.terrenosTrabalhados = new ArrayList<ITerreno>();
        this.mapa = mapa; // Inicializa o mapa cidade
        this.distMinCidades = 2;
        this.distMaxAloc = 3;
        this.recursos = new GestorRecursos();
        this.civilizacao = civilizacao;
        this.unidades = new GestorUnidades(this, mapa);
    }
    /**
     * Verifica se um terreno está dentro da distância máxima de trabalho da cidade.
     *
     * @param x Coordenada X do terreno.
     * @param y Coordenada Y do terreno.
     * @return true se o terreno está dentro da distância máxima, false caso contrário.
     */
    private boolean verificaDistTrabalho(int x, int y) {
        //Obter resultado positivo para a comparação
        int distanciaX = Math.abs(posX - x);
        int distanciaY = Math.abs(posY - y);
        // Verifica se está dentro da distância máxima
        if (distanciaX > distMaxAloc || distanciaY > distMaxAloc) {
            GestorMensagens.mostrarMensagem("Terreno fora dos limites da cidade.");
            return false;
        } else {
            return true;
        }
    }
    /**
     * Verifica se a distância entre esta cidade e outra cidade é maior ou igual à distância mínima permitida.
     *
     * @param outraCidade Cidade a ser comparada.
     * @return true se a distância é válida, false caso contrário.
     */
    private boolean verificarDistancia(Cidade outraCidade) {
        int distanciaX = Math.abs(this.posX - outraCidade.getPosX());
        int distanciaY = Math.abs(this.posY - outraCidade.getPosY());
        return distanciaX >= distMinCidades && distanciaY >= distMinCidades;
    }
    /**
     * Grava o estado da cidade num arquivo.
     *
     * @param writer Instância de BufferedWriter para gravar os dados.
     * @throws IOException Caso ocorra erro ao gravar no arquivo.
     */
    public void gravaDados(BufferedWriter writer) throws IOException {
        writer.write("Defesa: " + this.getDefesa());
        writer.newLine();
        writer.write("Populacao Total: " + this.getPopulacao().getTotal());
        writer.newLine();
        writer.write("Trabalhadores: " + this.getPopulacao().getTrabalhadores());
        writer.newLine();
        writer.write("Reserva de comida: " + recursos.getReservaComida());
        writer.newLine();
        // Gravar informações dos recursos
        gravaRecursos(writer);
        // Gravar terrenos trabalhados
        gravaTerrenosTrab(writer);
        // Gravar informações das unidades
        gravaUnidades(writer);
        
        writer.write("Cidade");
        writer.newLine();
    }
    /**
     * Grava os recursos da cidade no arquivo.
     *
     * @param writer Instância de BufferedWriter para gravar os recursos.
     * @throws IOException Caso ocorra erro ao gravar no arquivo.
     */
    private void gravaRecursos(BufferedWriter writer) throws IOException{
        writer.write("Recursos:");
        writer.newLine();
        for (String tipoRecurso : recursos.getRecursos().keySet()) {
            writer.write(tipoRecurso + ": " + recursos.getQuantidade(tipoRecurso));
            writer.newLine();
        }
    }
    /**
     * Grava os terrenos trabalhados pela cidade no arquivo.
     *
     * @param writer Instância de BufferedWriter para gravar os terrenos trabalhados.
     * @throws IOException Caso ocorra erro ao gravar no arquivo.
     */
    private void gravaTerrenosTrab(BufferedWriter writer) throws IOException{
        writer.write("Terrenos:\nTerrenos Trabalhados:");
        writer.newLine();
        for (ITerreno terreno : this.terrenosTrabalhados) {
            writer.write(" -, " + terreno.getX() + ", " + terreno.getY() + ", " + 
                    terreno.getCidadaosTrabalhando());
            writer.newLine();
        }
    }
    /**
     * Grava as unidades da cidade no arquivo.
     *
     * @param writer Instância de BufferedWriter para gravar as unidades.
     * @throws IOException Caso ocorra erro ao gravar no arquivo.
     */
    private void gravaUnidades(BufferedWriter writer) throws IOException{
        writer.write("Unidades:");
        writer.newLine();
        HashMap<String, List<IUnidade>> colecaoUnidades = unidades.getUnidades();
        for (Map.Entry<String, List<IUnidade>> entry : colecaoUnidades.entrySet()) {
            String tipoUnidade = entry.getKey();
            List<IUnidade> listaUnidades = entry.getValue();
            writer.write("Tipo de Unidade: " + tipoUnidade);
            writer.newLine();
            for (IUnidade unidade : listaUnidades) {
                writer.write(" -, " + unidade.getPosX() + ", " + unidade.getPosY() + ", "
                        + "Estado: " + (unidade.isOcupada() ? "true" : "false") + ", "
                            + "Defesa: " + unidade.getDefesa());
                writer.newLine();
            }
        }
    }
    /**
     * Processa uma linha do estado da cidade para restaurar os atributos e associações.
     *
     * @param linha Linha atual do estado da cidade.
     * @param reader Instância de BufferedReader para leitura do arquivo de estado.
     * @throws IOException Caso ocorra erro na leitura do arquivo.
     */
    public void processarLinhaEstado(String linha, BufferedReader reader) throws IOException {
        if (linha.startsWith("Defesa:")) {
            this.defesa = Integer.parseInt(linha.split(":")[1].trim());
        } else if (linha.startsWith("Populacao Total:")) {
            this.populacao.setTotal(Integer.parseInt(linha.split(":")[1].trim()));
        } else if (linha.startsWith("Trabalhadores:")) {
            this.populacao.setTrabalhadores(Integer.parseInt(linha.split(":")[1].trim()));
        } else if (linha.startsWith("Reserva de comida:")) {
            this.recursos.setReservaComida(Integer.parseInt(linha.split(":")[1].trim()));
        } else if (linha.startsWith("Recursos:")) {
            do {
                if (linha.equals("Recursos:")) {linha = reader.readLine();}
                carregaRecursos(linha);
                if (!linha.equals("Terrenos:")) {linha = reader.readLine();} 
            } while(!linha.equals("Terrenos:"));
        } else if (linha.startsWith("Terrenos Trabalhados:")) {
            do {
                if (linha.equals("Terrenos Trabalhados:")) {linha = reader.readLine();}
                carregaTerrenosTrab(linha);
                if (!linha.equals("Unidades:")){linha = reader.readLine();}
            } while(!linha.equals("Unidades:"));
        } else if (linha.startsWith("Tipo")) {
            do {
                String tipoUnidade = linha.split(":")[1].trim();
                List<IUnidade> listaUnidades = new ArrayList<>();
                linha = reader.readLine();
                while (linha.startsWith(" -")) {
                    carregaUnidades(linha, tipoUnidade, listaUnidades);
                    linha = reader.readLine();
                }                 
            } while (!linha.equals("Cidade"));
            carregaAcoes(reader);
        }
    }
    /**
     * Carrega as ações agendadas para o próximo turno.
     *
     * @param reader Instância de BufferedReader para leitura do arquivo de estado.
     * @throws IOException Caso ocorra erro na leitura do arquivo.
     */
    private void carregaAcoes(BufferedReader reader) throws IOException{
        reader = new BufferedReader(new FileReader("saveMovimentos.txt"));
        GestorMovimento.carregarMovimentos(reader, this.unidades.getUnidades());
        reader.close();
        reader = new BufferedReader(new FileReader("saveConstrucoes.txt"));
        GestorConstrucao.carregarConstrucoes(reader, this.unidades.getUnidades());
        reader.close();
    }
    /**
     * Carrega os recursos produzidos pela cidade
     *
     * @param linha Linha atual do estado da cidade.
     */
    private void carregaRecursos(String linha) {
        String[] recurso = linha.split(":");
        if (recurso.length == 2) {
            String tipo = recurso[0].trim();
            int quantidade = Integer.parseInt(recurso[1].trim());
            this.recursos.adicionarRecurso(tipo, quantidade);
        }
    }
    /**
     * Carrega as unidades da cidade.
     *
     * @param linha Linha atual do estado da cidade.
     * @param tipoUnidade Tipo de unidade para carregar.
     * @param listaUnidades lista com as unidades.
     */
    private void carregaUnidades(String linha, String tipoUnidade, List<IUnidade> listaUnidades) {
        String[] partes = linha.split(", ");
        int x = Integer.parseInt(partes[1].trim());
        int y = Integer.parseInt(partes[2].trim());
        boolean ocupada = Boolean.parseBoolean(partes[3].substring(partes[3].indexOf(": ") 
                + 2).trim());
        int defesaUnidade = Integer.parseInt(partes[4].split(": ")[1]);
        // Cria unidade com base no tipo
        IUnidade unidade = switch (tipoUnidade) {
                    case "COLONO" -> new UnidadeColono(x, y, this);
                    case "MILITAR" -> new UnidadeMilitar(x, y, this);
                    case "CONSTRUTOR" -> new UnidadeConstrutor(x, y, this);
                    default -> null;
                    };
        if (unidade != null) {
            unidade.setOcupada(ocupada);
            unidade.setDefesa(defesaUnidade);
            listaUnidades.add(unidade);
        }
        this.unidades.getUnidades().put(tipoUnidade, listaUnidades);
    }
    /**
     * Carrega os Terrenos trabalhados na cidade
     *
     * @param linha Linha atual do estado da cidade.
     */
    private void carregaTerrenosTrab(String linha) {
        if (linha.startsWith(" -")) {
            String[] partes = linha.split(", ");
            int x = Integer.parseInt(partes[1].trim());
            int y = Integer.parseInt(partes[2].trim());
            ITerreno terreno = mapa.getTerreno(x, y); // Obtém o terreno do mapa
            if (terreno != null) {
                terrenosTrabalhados.add(terreno);
                terreno.adicionarCidadaos(Integer.parseInt(partes[3].trim()));
            }
        }
        
    }
    /**
     * Processa as necessidades de consumo e produção de recursos da cidade.
     */
    public void necessidadesEProducao() {
        GestorMensagens.mostrarMensagem("\t*** Cidade: " + nome + " ***");
        // Se a população for zero, não há consumo
        if (populacao.getTotal() == 0) {
            GestorMensagens.mostrarMensagem("\nSem populacao para alimentar.");
            return;
        }
        // Calcula a produção total de comida nos terrenos trabalhados
        recursos.calcularProducao(this);

        int comidaGerada = recursos.getQuantidade("Alimento");
        int comidaConsumida = populacao.getTotal()*recursos.getAliNecessario();
        int excedente = comidaGerada - comidaConsumida;        
        // Atualiza produção de recursos
        System.out.println(recursos); 
        if (comidaGerada >= comidaConsumida) {
            // Comida suficiente para toda a população
            recursos.consumirRecurso("Alimento", comidaConsumida); // Consome a quantidade necessária
            GestorMensagens.mostrarMensagem("Toda a populacao foi alimentada.");
            // Adiciona o excedente à reserva             
            recursos.setReservaComida(excedente);
            if (recursos.getReservaComida() == 0) {recursos.mostraPopulacao(populacao);} 
        } else { if (recursos.getReservaComida() >= Math.abs(excedente)) {
            // Comida insuficiente: retirar da reserva            
            recursos.setReservaComida(excedente);
            GestorMensagens.mostrarMensagem("Comida gerada insuficiente! Recorreu-se a reserva." 
                    + recursos.getReservaComida());
            if (recursos.getReservaComida() == 0) {recursos.mostraPopulacao(populacao);}                
            } else {
                recursos.resetReservaComida(populacao);
            }
        }
        recursos.verificaReserva(this, populacao);
    }
    /**
     * Permite alocar os cidadãos para os terrenos.
     *
     * @param terreno Terreno para o qual os cidadãos vão ser alocados.
     * @param quantidade Quantidade de cidadãos a alocar
     */
    public void alocarCidadao(ITerreno terreno, int quantidade) {
        if (!terreno.toString().equals("Cidade")) {
            terreno.adicionarCidadaos(quantidade); // Associa os cidadãos ao terreno
            terrenosTrabalhados.add(terreno);
            populacao.setTrabalhadores(quantidade); // Reduz a população disponível
            GestorMensagens.mostrarMensagem("Numero de cidadaos alocados para " + terreno 
                + " em (" + terreno.getX() + ", " + terreno.getY() + "): " + quantidade);
        } else {
            GestorMensagens.mostrarMensagem("Terreno invalido na posicao (" + terreno.getX()
                + ", " + terreno.getY() + ").");
        }
    }
    /**
     * Permite retirar os cidadãos dos terrenos.
     *
     * @param terreno Terreno de onde os cidadãos vão ser retirados.
     * @param quantidade Quantidade de cidadãos a desalocar
     */
    public void desalocarCidadao(ITerreno terreno, int quantidade) {
        if (terreno.getCidadaosTrabalhando() >= quantidade) {
            terreno.removerCidadaos(quantidade);
            populacao.setTrabalhadores(-quantidade); // Retorna os cidadãos à população disponível
            GestorMensagens.mostrarMensagem("Numero de cidadaos removidos de " + terreno + " em "
                + "(" + terreno.getX() + ", " + terreno.getY() + "): " + quantidade);
            // Remove o terreno da lista de trabalhados se não houver mais cidadãos
            if (terreno.getCidadaosTrabalhando() == 0) {
                terrenosTrabalhados.remove(terreno);
            } 
        } else {
            GestorMensagens.mostrarMensagem("Numero de trabalhadores inferior ao introduzido.");
        }    
    }  
    @Override
    /**
     * Reduz a defesa da cidade.
     *
     * @param quantidade quantidade de dano recebido
     * @param atacante civilização que está a atacar
     */
    public void receberDano(int quantidade, Civilizacao atacante) {
        defesa -= quantidade;
        if (defesa <= 0) {
            conquistada(atacante);
        }
    }

    @Override
    /**
     * Verifica se a cidade está derrotada.
     * @return true se a cidade está derrotada, false pelo contrário
     */
    public boolean estaDerrotada() {
        return this.defesa <= 0;
    }

    /**
    * Conquista a cidade e atualiza sua civilização.
    *
    * Este método remove a cidade da sua civilização atual, atualiza a 
    * civilização para a nova e reinicia a população da cidade para zero. 
    * Também atualiza o mapa e notifica os jogadores sobre a conquista.
    *
    * @param novaCivilizacao a nova civilização que conquista a cidade
    */
   public void conquistada(Civilizacao novaCivilizacao) {
       // Remover a cidade da civilização atual
       if (this.civilizacao != null) {
           this.civilizacao.getCidades().remove(this);
       }
       // Atualizar a civilização
       this.civilizacao = novaCivilizacao;
       ((ITerrenoCidade)mapa.getTerreno(posX, posY)).setCidadeJogador();
       mapa.getTerreno(posX, posY).removerAlvo(this);
       novaCivilizacao.adicionarCidade(this);
       // Eliminar a população atual
       this.populacao = new Populacao(this, 0); // População reiniciada a zero
       GestorMensagens.mostrarMensagem("Cidade " + this.nome + " foi conquistada pela civilizacao " 
                                       + novaCivilizacao.getNome() + "!");
   }

   @Override
   /**
    * Obtém uma descrição da cidade.
    *
    * Este método retorna uma string contendo o nome da cidade, o nome de sua 
    * civilização, sua posição no mapa e seu valor de defesa atual.
    *
    * @return uma descrição em string da cidade
    */
   public String getDescricao() {
       String texto = "Cidade " + nome + " " + civilizacao.getNome();
       texto += " (" + posX + ", " + posY + ")";
       texto += "\nDefesa: " + defesa;
       return texto;
   }

   @Override
   /**
    * Obtém o valor atual de defesa da cidade.
    *
    * @return o valor atual de defesa
    */
   public int getDefesa() {
       return defesa;
   }

   @Override
   /**
    * Define o valor de defesa da cidade.
    *
    * @param defesa o novo valor de defesa a ser definido
    */
   public void setDefesa(int defesa) {
       this.defesa = defesa;
   }

   @Override
   /**
    * Obtém a força de combate da cidade.
    *
    * @return a força de combate
    */
   public int getForcaCombate() {
       return forcaCombate;
   }

   @Override
   /**
    * Obtém a posição X da cidade no mapa.
    *
    * @return a posição X
    */
   public int getPosX() {
       return posX;
   }

   @Override
   /**
    * Obtém a posição Y da cidade no mapa.
    *
    * @return a posição Y
    */
   public int getPosY() {
       return posY;
   }

   /**
    * Obtém o nome da cidade.
    *
    * @return o nome da cidade
    */
   public String getNome() {
       return nome;
   }

   /**
    * Obtém a civilização que possui a cidade.
    *
    * @return a civilização da cidade
    */
   public Civilizacao getCivilizacao() {
       return civilizacao;
   }

   /**
    * Obtém o gestor de unidades associado à cidade.
    *
    * @return o gestor de unidades
    */
   public GestorUnidades getGestorUnidades() {
       return unidades;
   }

   /**
    * Define o nome da cidade.
    *
    * @param nome o novo nome da cidade
    */
   public void setNome(String nome) {
       this.nome = nome;
   }

   /**
    * Obtém a população atual da cidade.
    *
    * @return o objeto população da cidade
    */
   public Populacao getPopulacao() {
       return populacao;
   }

   /**
    * Obtém uma lista de terrenos trabalhados pela cidade.
    *
    * @return uma lista de terrenos trabalhados pela cidade
    */
   public ArrayList<ITerreno> getTerrenosTrabalhados() {
       return new ArrayList<ITerreno>(terrenosTrabalhados);
   }
    /**
    * Obtém o gestor de recursos associado à cidade.
    *
    * @return o gestor de recursos da cidade
    */
   public GestorRecursos getGestorRecursos() {
       return recursos;
   }

   /**
    * Obtém a distância mínima entre cidades.
    *
    * Este método retorna a distância mínima que deve ser respeitada entre 
    * as cidades.
    *
    * @return a distância mínima entre cidades
    */
   public static int getDistanciaMinima() {
       return distMinCidades;
   }

   /**
    * Define a distância mínima entre cidades.
    *
    * Este método permite definir uma nova distância mínima que deve ser 
    * respeitada entre as cidades.
    *
    * @param novaDistancia a nova distância mínima a ser definida
    */
   public static void setDistanciaMinima(int novaDistancia) {
       distMinCidades = novaDistancia;
   }

   /**
    * Obtém a distância máxima de alocação.
    *
    * Este método retorna a distância máxima permitida para alocação de 
    * recursos ou unidades.
    *
    * @return a distância máxima de alocação
    */
   public static int getDistMaxAloc() {
       return distMaxAloc;
   }

   /**
    * Define a distância máxima de alocação.
    *
    * Este método permite definir uma nova distância máxima que pode ser 
    * utilizada para alocação de recursos ou unidades.
    *
    * @param novaDistancia a nova distância máxima a ser definida
    */
   public static void setDistMaxAloc(int novaDistancia) {
       distMaxAloc = novaDistancia;
   }

   /**
    * Retorna uma representação em string da cidade.
    *
    * Este método fornece uma descrição detalhada da cidade, incluindo seu 
    * nome, posição no mapa, recursos disponíveis, população e defesa.
    *
    * @return uma string representando a cidade
    */
   @Override
   public String toString() {
       String text = "\nNome: " + getNome();
       text += "\nPosicao: (" + posX + ", " + posY + ")";
       text += "\nRecursos disponiveis:";
       text += "\nAlimento-> " + recursos.getQuantidade("Alimento");
       text += "\nProducao-> " + recursos.getQuantidade("Producao");
       text += "\nOuro-> " + recursos.getQuantidade("Ouro");
       text += "\nPopulacao: " + populacao.getTotal();
       text += "\nReserva de Comida: " + recursos.getReservaComida();
       text += "\nDefesa: " + defesa;

       return text;
   }
}

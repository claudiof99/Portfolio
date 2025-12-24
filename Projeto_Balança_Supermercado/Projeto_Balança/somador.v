`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    16:32:34 12/21/2023 
// Design Name: 
// Module Name:    somador 
// Project Name: 
// Target Devices: 
// Tool versions: 
// Description: 
//
// Dependencies: 
//
// Revision: 
// Revision 0.01 - File Created
// Additional Comments: 
//
//////////////////////////////////////////////////////////////////////////////////
module somador_preco_peso (
  input wire clk,        // Sinal de clock
  input wire rst,        // Sinal de reset
  input wire fim_compra,  // Sinal de fim de compra
  input wire [10:0] preco_produto, //preço calculado produto
  input wire [10:0] peso_produto, // peso produto
  
  input wire taxa, // Sinal de aplicação de taxa
  
  output wire [10:0] soma_final, // Resultado preço acumulado
  output wire [10:0] soma_peso, // Resultado peso acumulado
  output reg emissao_talao, // Sinal emissao talao
  output reg [4:0] valor_taxa // Indicaçao da taxa aplicada
);

reg [10:0] soma_atual;
reg [10:0] peso_atual;

always @(posedge clk or posedge rst) begin
    if (rst) begin
        soma_atual <= 11'b0;
		  peso_atual <= 11'b0;
		  emissao_talao <= 0;
		  valor_taxa <= 0;
    end else begin
        if (fim_compra) begin  // A compra foi finalizada.
            emissao_talao <= 1; // Emite o talao
            if (taxa) begin // Aplica a taxa de 27%
                soma_atual <= (soma_atual * 127) / 100;
					 valor_taxa <=5'b11011; // Indica o valor da taxa
            end
		  end else begin
		      soma_atual <= soma_atual + preco_produto; // vai somando os preços
				peso_atual <= peso_atual + peso_produto; // vai somando os pesos
				emissao_talao <= 0; 
            valor_taxa <= 0;			
        end
    end
end

assign soma_final = soma_atual;
assign soma_peso = peso_atual;

endmodule

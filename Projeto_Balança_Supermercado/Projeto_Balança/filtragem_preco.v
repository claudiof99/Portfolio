`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    19:08:20 11/09/2023 
// Design Name: 
// Module Name:    ent_preco 
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
module filtragem_preco(
  input wire [1:0] produto,
  input wire [8:0] preco_banana,
  input wire [8:0] preco_maracuja,
  input wire [8:0] preco_tangerina,
  
  output reg [8:0] preco_fil
);

  reg [8:0] preco_bruto;
  
  always @* begin
    case (produto)
      2'b01: preco_bruto = preco_banana;
      2'b10: preco_bruto = preco_maracuja;
      2'b11: preco_bruto = preco_tangerina;
      default: preco_bruto = 0; // Valor padrão caso o produto não seja 1, 2 ou 3
    endcase
  end

  always @* begin 
	if (preco_bruto <= 9'b111110100) begin // Se o preço bruto for inferior ou igual a 500 cêntimos
      preco_fil = preco_bruto; // O preço filtrado é igual ao preço bruto
    
	 end else begin // Para o resto
      preco_fil = 9'b111110100; // O preço filtrado é igual a 500 cêntimos
    end
  end

endmodule

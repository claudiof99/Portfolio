`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    21:44:11 11/09/2023 
// Design Name: 
// Module Name:    saida_preco_calculado 
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
module multiplicador_11_9_bin_11(
  input [10:0] entrada_11b,
  input [8:0] entrada_9b,
  
  output reg [10:0] saida_11b
);

  always @(entrada_11b, entrada_9b)
  begin
    // Multiplica as entradas de 11 e 9 bits
	 // Divide por 1000, deslocando a vírgula decimal três posições para a esquerda
    saida_11b = (entrada_11b * entrada_9b) / 1000;
  end

endmodule

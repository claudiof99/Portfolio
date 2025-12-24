`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    16:59:37 11/10/2023 
// Design Name: 
// Module Name:    separador_BCDg 
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
module separador_BCD_4_12(
    input [15:0] BCD_total,  // Entrada de 16 bits do BCD
    output reg [3:0] BCD_inteiro,  // Saída de 4 bits para a parte inteira do BCD
    output reg [11:0] BCD_fracionario  // Saída de 12 bits para a parte fracionária do BCD
);

    always @(BCD_total) begin
        // Separa o BCD em parte inteira e decimal
        BCD_inteiro = BCD_total[15:12];  // Atribui os bits de 16 a 13 do BCD_total a BCD_inteiro
        BCD_fracionario = BCD_total[11:0];  // Atribui os bits de 12 a 0 do BCD_total a BCD_fracionario
    end

endmodule

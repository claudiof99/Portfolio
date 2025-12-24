`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    17:33:40 11/10/2023 
// Design Name: 
// Module Name:    separador_BCD_8_8 
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
module separador_BCD_8_8(
    input [15:0] BCD_total, // Entrada de 16 bits do BCD
    output reg [7:0] BCD_inteiro, // Saída de 8 bits para a parte inteira do BCD
    output reg [7:0] BCD_fracionario // Saída de 8 bits para a parte fracionária do BCD
);

    always @(BCD_total) begin
        // Separa o BCD em parte inteira e decimal
        BCD_inteiro = BCD_total[15:8]; // Atribui os bits de 16 a 9 do BCD_total a BCD_inteiro
        BCD_fracionario = BCD_total[7:0]; // Atribui os bits de 8 a 0 do BCD_total a BCD_fracionario
    end

endmodule

`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    19:32:52 11/09/2023 
// Design Name: 
// Module Name:    saida_peso 
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

module bin2bcd_11_16(
   input [10:0] bin,
   output reg [15:0] bcd
);

integer i;

always @(bin) begin
    bcd = 0; // Inicializa o número BCD como zero

    for (i = 0; i < 11; i = i + 1) begin
        // Se algum dígito BCD for maior ou igual a 5, adiciona 3
        if (bcd[3:0] >= 5) bcd[3:0] = bcd[3:0] + 3;
        if (bcd[7:4] >= 5) bcd[7:4] = bcd[7:4] + 3;
        if (bcd[11:8] >= 5) bcd[11:8] = bcd[11:8] + 3;
        if (bcd[15:12] >= 5) bcd[15:12] = bcd[15:12] + 3;

        // Realiza um deslocamento de um bit para a esquerda e adiciona o próximo bit da entrada
        bcd = {bcd[14:0], 1'b0};
        bcd[0] = bin[10 - i];
    end
end

endmodule




// Copyright 1986-2018 Xilinx, Inc. All Rights Reserved.
// --------------------------------------------------------------------------------
// Tool Version: Vivado v.2018.2 (win64) Build 2258646 Thu Jun 14 20:03:12 MDT 2018
// Date        : Sun Oct 12 19:48:32 2025
// Host        : DESKTOP-9RFJ3A6 running 64-bit major release  (build 9200)
// Command     : write_verilog -mode timesim -nolib -sdf_anno true -force -file {D:/XC/4th
//               kurs/POCP/Lab3/Lab3.sim/sim_1/impl/timing/xsim/RS_beh_Test_time_impl.v}
// Design      : RS_beh
// Purpose     : This verilog netlist is a timing simulation representation of the design and should not be modified or
//               synthesized. Please ensure that this netlist is used with the corresponding SDF file.
// Device      : xc7a100tcsg324-1
// --------------------------------------------------------------------------------
`timescale 1 ps / 1 ps
`define XIL_TIMING

(* ECO_CHECKSUM = "919a4b4c" *) 
(* NotValidForBitStream *)
module RS_beh
   (S,
    R,
    Q,
    nQ);
  input S;
  input R;
  output Q;
  output nQ;

  wire Q;
  wire Q_OBUF;
  wire R;
  wire R_IBUF;
  wire S;
  wire S_IBUF;
  wire nQ;
  wire nQ_OBUF;

initial begin
 $sdf_annotate("RS_beh_Test_time_impl.sdf",,,,"tool_control");
end
  OBUF Q_OBUF_inst
       (.I(Q_OBUF),
        .O(Q));
  LUT3 #(
    .INIT(8'h0E)) 
    Q_OBUF_inst_i_1
       (.I0(S_IBUF),
        .I1(Q_OBUF),
        .I2(R_IBUF),
        .O(Q_OBUF));
  IBUF R_IBUF_inst
       (.I(R),
        .O(R_IBUF));
  IBUF S_IBUF_inst
       (.I(S),
        .O(S_IBUF));
  OBUF nQ_OBUF_inst
       (.I(nQ_OBUF),
        .O(nQ));
  LUT2 #(
    .INIT(4'h1)) 
    nQ_OBUF_inst_i_1
       (.I0(Q_OBUF),
        .I1(S_IBUF),
        .O(nQ_OBUF));
endmodule
`ifndef GLBL
`define GLBL
`timescale  1 ps / 1 ps

module glbl ();

    parameter ROC_WIDTH = 100000;
    parameter TOC_WIDTH = 0;

//--------   STARTUP Globals --------------
    wire GSR;
    wire GTS;
    wire GWE;
    wire PRLD;
    tri1 p_up_tmp;
    tri (weak1, strong0) PLL_LOCKG = p_up_tmp;

    wire PROGB_GLBL;
    wire CCLKO_GLBL;
    wire FCSBO_GLBL;
    wire [3:0] DO_GLBL;
    wire [3:0] DI_GLBL;
   
    reg GSR_int;
    reg GTS_int;
    reg PRLD_int;

//--------   JTAG Globals --------------
    wire JTAG_TDO_GLBL;
    wire JTAG_TCK_GLBL;
    wire JTAG_TDI_GLBL;
    wire JTAG_TMS_GLBL;
    wire JTAG_TRST_GLBL;

    reg JTAG_CAPTURE_GLBL;
    reg JTAG_RESET_GLBL;
    reg JTAG_SHIFT_GLBL;
    reg JTAG_UPDATE_GLBL;
    reg JTAG_RUNTEST_GLBL;

    reg JTAG_SEL1_GLBL = 0;
    reg JTAG_SEL2_GLBL = 0 ;
    reg JTAG_SEL3_GLBL = 0;
    reg JTAG_SEL4_GLBL = 0;

    reg JTAG_USER_TDO1_GLBL = 1'bz;
    reg JTAG_USER_TDO2_GLBL = 1'bz;
    reg JTAG_USER_TDO3_GLBL = 1'bz;
    reg JTAG_USER_TDO4_GLBL = 1'bz;

    assign (strong1, weak0) GSR = GSR_int;
    assign (strong1, weak0) GTS = GTS_int;
    assign (weak1, weak0) PRLD = PRLD_int;

    initial begin
	GSR_int = 1'b1;
	PRLD_int = 1'b1;
	#(ROC_WIDTH)
	GSR_int = 1'b0;
	PRLD_int = 1'b0;
    end

    initial begin
	GTS_int = 1'b1;
	#(TOC_WIDTH)
	GTS_int = 1'b0;
    end

endmodule
`endif

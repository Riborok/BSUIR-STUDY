----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 19:18:34
-- Design Name: 
-- Module Name: SREGn_struct - Structural
-- Project Name: 
-- Target Devices: 
-- Tool Versions: 
-- Description: 
-- 
-- Dependencies: 
-- 
-- Revision:
-- Revision 0.01 - File Created
-- Additional Comments:
-- 
----------------------------------------------------------------------------------


library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity SREGn_struct is
    generic (N : integer := 8);
    port (
        Sin  : in std_logic;
        SE   : in std_logic;
        CLK  : in std_logic;
        RST  : in std_logic;
        Pout : out std_logic_vector(N-1 downto 0)
    );
end SREGn_struct;

architecture Structural of SREGn_struct is
    component FDCE_trigger
        port (
            D   : in std_logic;
            CE  : in std_logic;
            CLR : in std_logic;
            CLK : in std_logic;
            Q   : out std_logic
        );
    end component;

    signal Q_int : std_logic_vector(N-1 downto 0);
begin
    stage_0 : FDCE_trigger port map(
        D   => Sin,
        CE  => SE,
        CLR => RST,
        CLK => CLK,
        Q   => Q_int(0)
    );
    
    gen_shift : for i in 1 to N-1 generate
        stage_i : FDCE_trigger port map(
            D   => Q_int(i-1),
            CE  => SE,
            CLR => RST,
            CLK => CLK,
            Q   => Q_int(i)
        );
    end generate;

    Pout <= Q_int;
end Structural;

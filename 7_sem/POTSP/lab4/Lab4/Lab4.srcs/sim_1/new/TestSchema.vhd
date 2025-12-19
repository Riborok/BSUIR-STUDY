library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity TestSchema is
--  Port ( );
end TestSchema;

architecture Behavioral of TestSchema is
    constant CLK_PERIOD  : time := 10 ns;
    
    constant GEN_N       : integer := 7;
    constant GEN_POLYNOM : std_logic_vector(GEN_N-1 downto 0) := "1000001";
    
    constant ANL_N       : integer := 8;
    constant ANL_POLYNOM : std_logic_vector(ANL_N downto 0) := "111110101";
    
    component combination is
        port (
            Din: in std_logic_vector(6 downto 0); 
            Dout: out std_logic
        );
    end component;
    
    component lfsr_ext is
        generic (
            N: integer; 
            Polynom: std_logic_vector
        );
        port (
            CLK, RST: in std_logic; 
            Q: out std_logic_vector(N-1 downto 0)
        );
    end component;
    
    component signature_analyzer is
        generic (
            N: integer; 
            Polynom: std_logic_vector
        );
        port (
            CLK, RST, Din: in std_logic; 
            Sign: out std_logic_vector(N-1 downto 0)
        );
    end component;
    
    signal CLK: std_logic := '0';
    signal RST: std_logic;
    signal gen_to_cut: std_logic_vector(GEN_N-1 downto 0);
    signal cut_to_anl: std_logic;
    signal final_signature: std_logic_vector(ANL_N-1 downto 0);
begin
    uut_comb: combination
         port map (
            Din => gen_to_cut, 
            Dout => cut_to_anl
         );
    
    uut_lfsr: lfsr_ext
         generic map (
            N => GEN_N, 
            Polynom => GEN_POLYNOM
         )
         port map (
            CLK => CLK, 
            RST => RST, 
            Q => gen_to_cut
         );
             
    uut_analyzer: signature_analyzer
         generic map (
            N => ANL_N, 
            Polynom => ANL_POLYNOM
         )
         port map (
            CLK => CLK, 
            RST => RST, 
            Din => cut_to_anl, 
            Sign => final_signature
         );
         
     clk_process: process
     begin
         CLK <= '0'; 
         wait for CLK_PERIOD / 2;
         CLK <= '1'; 
         wait for CLK_PERIOD / 2;
     end process;
     
     stim_proc: process
     begin
         RST <= '1';
         wait for CLK_PERIOD * 3;
         RST <= '0';
         
         wait for CLK_PERIOD * 128;
             
         report "The end" severity failure;
         wait;
     end process;
     
end Behavioral;

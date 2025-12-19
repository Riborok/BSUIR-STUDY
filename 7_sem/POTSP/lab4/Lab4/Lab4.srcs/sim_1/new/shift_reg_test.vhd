library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity shift_reg_test is
--  Port ( );
end shift_reg_test;

architecture Behavioral of shift_reg_test is
    constant N_BITS: integer := 8;
    constant CLK_PERIOD: time := 10 ns;
    
    component shift_reg_beh is
        generic (
            N: integer := N_BITS
        );
        port (
            CLK, RST, SE, Sin: in std_logic;
            Pout: out std_logic_vector(N_BITS-1 downto 0)
        );
    end component;

    component shift_reg_struct is
        generic (
            N: integer := N_BITS
        );
        port (
            CLK, RST, SE, Sin: in std_logic;
            Pout: out std_logic_vector(N_BITS-1 downto 0)
        );
    end component;
    
    signal CLK: std_logic := '0';
    signal RST: std_logic := '0';
    signal SE: std_logic := '0';
    signal Sin: std_logic := '0';
    
    signal Pout_beh: std_logic_vector(N_BITS-1 downto 0);
    signal Pout_struct: std_logic_vector(N_BITS-1 downto 0);
    
    signal Error: std_logic;
begin
    uut_beh : shift_reg_beh
        port map (
            CLK => CLK,
            RST => RST,
            SE => SE,
            Sin => Sin,
            Pout => Pout_beh
        );

    uut_struct : shift_reg_struct
        port map (
            CLK => CLK,
            RST => RST,
            SE => SE,
            Sin => Sin,
            Pout => Pout_struct
        );
        
    Error <= '1' when (Pout_beh /= Pout_struct) else '0';
    
    clk_process : process
    begin
        CLK <= '0';
        wait for CLK_PERIOD / 2;
        CLK <= '1';
        wait for CLK_PERIOD / 2;
    end process clk_process;
    
    stim_proc : process
    begin
        RST <= '1';
        wait for 15 ns;
        RST <= '0';
        wait for 5 ns;

        SE  <= '0';
        Sin <= '1';
        wait for CLK_PERIOD * 2;

        SE <= '1';
        Sin <= '1'; wait for CLK_PERIOD;
        Sin <= '0'; wait for CLK_PERIOD;
        Sin <= '1'; wait for CLK_PERIOD;
        Sin <= '1'; wait for CLK_PERIOD;
        Sin <= '0'; wait for CLK_PERIOD;
        Sin <= '0'; wait for CLK_PERIOD;
        Sin <= '0'; wait for CLK_PERIOD;
        Sin <= '1'; wait for CLK_PERIOD;

        SE  <= '0';
        Sin <= '0';
        wait for CLK_PERIOD * 2;
        
        
        if Error = '1' then
            report "Behaviour and structural are not equal" severity failure;
        else
            RST <= '1';
            wait for CLK_PERIOD / 2;
            RST <= '0';
            report "Òhe end" severity failure;
        end if;
        
        wait;
    end process;
end Behavioral;

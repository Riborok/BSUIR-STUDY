%{
#include <stdio.h>
%}

%token STRING NUMBER BOOLEAN COLON COMMA LEFT_BRACE RIGHT_BRACE LEFT_BRACKET RIGHT_BRACKET MINUS PLUS SLASH ASTERISK

%%

json_document: json_value
    ;

json_value: STRING
    | NUMBER
    | BOOLEAN
    | json_object
    | json_array
    ;

json_object: LEFT_BRACE RIGHT_BRACE
    | LEFT_BRACE members RIGHT_BRACE
    ;

members: pair
    | pair COMMA members
    ;

pair: STRING COLON json_value
    ;

json_array: LEFT_BRACKET RIGHT_BRACKET
    | LEFT_BRACKET elements RIGHT_BRACKET
    ;

elements: json_value
    | json_value COMMA elements
    ;

%%

void yyerror(const char* s) {
    fprintf(stderr, "%s\n", s);
}

int main() {
    if (yyparse() == 0) {
        printf("JSON file is correct\n");
    }
    return 0;
}

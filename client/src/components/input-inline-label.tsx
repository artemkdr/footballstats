import { InputAddonProps, InputGroup, InputLeftAddon } from '@chakra-ui/react';

interface InputInlineLabelProps extends InputAddonProps {
    label: string;    
}

export const InputInlineLabel: React.FC<InputInlineLabelProps> = (props) => {
    const { children, label, ...rest } = props;
    return (
        <InputGroup {...rest}>
            <InputLeftAddon
                width="40%"
                whiteSpace="initial"
                textAlign={'right'}
                textOverflow="ellipsis"
                overflow={'hidden'}
                justifyContent="end"
                fontWeight="bold"
                textTransform="uppercase"
                fontSize="10px"
            >
                {label}
            </InputLeftAddon>
            {children}
        </InputGroup>
    );
};

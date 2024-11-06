import { Link as ChakraLink, LinkProps, Text } from '@chakra-ui/react';
import { Link as ReactRouterLink } from 'react-router-dom';

interface CustomLinkProps extends LinkProps {        
    link : string;
    text: string;
}

export const CustomLink: React.FC<CustomLinkProps> = (props) => {
    const { link, text, ...rest } = props;
    return (
        <ChakraLink as={ReactRouterLink} to={link} textDecoration={"underline"} {...rest}>
            <Text>{text}</Text>																
        </ChakraLink>
    )
}
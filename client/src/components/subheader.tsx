import { Heading, HeadingProps } from '@chakra-ui/react';

interface SubheaderProps extends HeadingProps {
    text: string;
}

export const Subheader: React.FC<SubheaderProps> = (props) => {
    const { text, ...rest } = props;
    return (
        <Heading as="h3" size="sm" marginTop={4} {...rest}>
            {text}
        </Heading>
    );
};

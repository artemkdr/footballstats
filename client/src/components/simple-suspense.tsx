interface LoaderProps {
    children?: React.ReactNode;    
    fallback?: React.ReactNode;
    emptyText?: string;
}

export const SimpleSuspense: React.FC<LoaderProps> = (props) => {
    const { children } = props;

    if (children === undefined) {
        return <>{props.fallback}</>;
    }
    if (Array.isArray(children) && children.length === 0) {
        return <>{props.emptyText}</>;
    }
    return (
        <>
            {children}
        </>
    );
}
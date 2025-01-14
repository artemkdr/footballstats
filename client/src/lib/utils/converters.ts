export const toInt = (value: unknown): number => {
    return typeof(value) == 'number' ? Math.round(value) : (parseInt(String(value)) || 0);
}

export const toNumber = (value: unknown): number => {
    return typeof(value) == 'number' ? value : (Number(String(value)) || 0.0);
}

export const toString = (value: unknown): string => {
    return String(value);
}
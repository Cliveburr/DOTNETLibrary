
export type ClassValue = string | undefined | null;

export const classNames = (...classes: ClassValue[]) => {
    return classes
        .filter(c => !!c)
        .join(" ");
}
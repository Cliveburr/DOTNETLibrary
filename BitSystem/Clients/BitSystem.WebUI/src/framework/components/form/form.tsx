
interface FormProps {
    children: React.ReactNode;
}

export const Form = (args: FormProps) => {

    return (
        <form>
            {args.children}
        </form>
    )
}
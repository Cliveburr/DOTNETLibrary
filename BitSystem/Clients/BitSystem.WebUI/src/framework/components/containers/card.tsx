import { classNames } from "../..";

interface FieldProps {
    children: React.ReactNode;
    header?: string;
    className?: string | undefined;
}

export const Card = ({ header, children, className }: FieldProps) => {
    
    if (header) {
        return (
            <div className={classNames(className, "card")}>
                <div className="card-header">
                    <p className="card-header-title">{header}</p>
                </div>
                <div className="card-content">
                    {children}
                </div>
            </div>
        )
   
    }

    return (
        <div className={classNames(className, "card")}>
            <div className="card-content">
                {children}
            </div>
        </div>
    )
}
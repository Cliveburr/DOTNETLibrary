

export const watch = <T>(value: T): WatchValue<T> => {
    return new WatchValue<T>(value);
}

export interface WatchDelegate<T> {
    (value: T, oldValue: T | null): void;
}

export class WatchValue<T> {

    public watchers: WatchDelegate<T>[];

    public constructor(
        private inValue: T
    ) {
        this.watchers = [];
    }

    public get value(): T {
        return this.inValue;
    }

    public set value(value: T) {
        const oldValue = this.inValue;
        this.inValue = value;
        this.watchers.forEach(w => w(value, oldValue))
    }

    public update(update: (value: T) => void, dispatch: boolean = true): void {
        update(this.inValue);
        if (dispatch) {
            this.watchers.forEach(w => w(this.inValue, null))
        }
    }

    public on(delegate: WatchDelegate<T>): WatchValue<T> {
        this.watchers.push(delegate);
        return this;
    }

    public off(delegate: WatchDelegate<T>): WatchValue<T> {
        const index = this.watchers.indexOf(delegate);
        if (index > -1) {
            this.watchers.splice(index, 1);
        }
        return this;
    }
}
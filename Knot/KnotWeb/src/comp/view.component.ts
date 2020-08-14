import { Component } from '@angular/core';

class Knot {
    name: string;
    childs?: Knot[];
}

@Component({
  templateUrl: 'view.component.html',
  styleUrls: ['view.component.scss']
})
export class ViewComponent {

    protected root: Knot;

    constructor() {
        this.root = {
            name: 'Deploys',
            childs: [
                { name: 'Deploy Um' },
                { name: 'Deploy dois' }
            ]
        }
    }
}

export class DropdownOptionModel{
    Id?:number;
    Text?:string;

    constructor(data: {Id:number, Text:string}){
        this.Id = data.Id;
        this.Text = data.Text;
    }
}
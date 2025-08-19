import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ColorApi, ColorGetAllResponse } from '../../../api/color.api';

@Component({
  selector: 'app-color-list',
  templateUrl: './color-list.component.html',
  standalone: false,
  styleUrls: ['./color-list.component.css']
})
export class ColorListComponent implements OnInit {
  colors: ColorGetAllResponse[] = [];

  constructor(
    private colorApi: ColorApi,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors() {
    this.colorApi.getAll().subscribe(colors => this.colors = colors);
  }

  editColor(color: ColorGetAllResponse) {
    this.router.navigate([color.id], { relativeTo: this.route });
  }

  createColor() {
    this.router.navigate(['new'], { relativeTo: this.route });
  }

  deleteColor(color: ColorGetAllResponse) {
    if(confirm(`Are you sure you want to delete ${color.name}?`)) {
      this.colorApi.delete(color.id).subscribe(() => this.loadColors());
    }
  }
}

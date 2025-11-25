import { Component, AfterViewInit } from '@angular/core';
import * as L from 'leaflet';

@Component({
  selector: 'app-leaflet-map',
  templateUrl: './leaflet-map.component.html',
  standalone: false,
  styleUrls: ['./leaflet-map.component.css']
})
export class LeafletMapComponent implements AfterViewInit {

  ngAfterViewInit(): void {
    const location: L.LatLngExpression = [43.35561395054432, 17.809472916646964];//Fixed location of FIT Mostar


    const map = L.map('map').setView(location, 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '© OpenStreetMap'
    }).addTo(map);

    L.marker(location).addTo(map)
      .bindPopup('Hrke Shop')
      .openPopup();
  }
}
